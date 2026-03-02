using NotificationService.Application.Notifications;
using NotificationService.Application.RetryQueue;
using NotificationService.Application.Settings;
using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;
using System.Threading.Channels;

namespace NotificationService.Application.Pipeline;

public class NotificationPipeline : IReconfigurable {
    private readonly CapacityLimiter _totalCapacityLimiter;

    private readonly Channel<NotificationEntry> _inbound;

    private readonly Dictionary<DeliveryChannel, IRetryQueue> _retryQueues;

    private readonly AutoResetEvent _autoResetEvent = new(false);

    private readonly ProviderLaneStore _laneStore;

    private readonly DomainEventDispatcher _dispatcher;

    private PipelineSettings _settings;

    public NotificationPipeline(
        Dictionary<DeliveryChannel, IRetryQueue> retryQueues,
        ProviderLaneStore laneStore,
        DomainEventDispatcher dispatcher,
        PipelineSettings initialSettings
    ) {
        _retryQueues = retryQueues;
        _inbound = Channel.CreateUnbounded<NotificationEntry>();
        _laneStore = laneStore;
        _dispatcher = dispatcher;
        _settings = initialSettings;
        _totalCapacityLimiter = new CapacityLimiter(initialSettings.TotalNotificationCapacity);
    }

    public void ApplySettings(PipelineSettings settings) {
        _settings = settings;

        if (settings.TotalNotificationCapacity != _totalCapacityLimiter.Capacity) {
            _totalCapacityLimiter.SetCapacity(settings.TotalNotificationCapacity);
        }
    }

    public bool TrySubmitNew(NotificationEntry entry) {
        bool reserved = _totalCapacityLimiter.TryReserve(1);

        if (!reserved) {
            return false;
        }

        bool written = _inbound.Writer.TryWrite(entry);

        if (!written) {
            _totalCapacityLimiter.Release(1);
            throw new InvalidOperationException("Could not write to unbound channel");
        }

        Wake();

        return true;
    }

    public void SubmitRetry(NotificationEntry entry) {
        DateTime now = DateTime.UtcNow;

        var channel = entry.Notification.Channel;
        var channelSettings = _settings.Channels[channel];

        if (entry.RetryCount >= channelSettings.MaxNumRetries) {
            using (var _ = entry.Lock()) {
                entry.State = null;
                entry.RetryAt = null;

                entry.Notification.MarkAsFailed($"Exceeded maximum retry limit of {channelSettings.MaxNumRetries} attempts", now);

                _dispatcher.Dispatch(entry.Notification.DomainEvents);
                entry.Notification.ClearDomainEvents();
            }

            _totalCapacityLimiter.Release(1);

            return;
        }

        DateTime retryAt = RetryPolicy.FindRetryTime(
            entry.RetryCount, now,
            channelSettings.BackoffMultiplier,
            channelSettings.InitialRetryDelay,
            channelSettings.MaxRetryDelay
        );

        if(retryAt >= entry.ExpiresAt) {
            SettleEntry(entry, now, false, "Notification time to live expires before the notification can be sent");
            return;
        }

        using (var _ = entry.Lock()) {
            entry.State = ProcessingState.QueuedForProviderRetry;
            entry.RetryCount += 1;
            entry.RetryAt = retryAt;
        }

        _retryQueues[channel].Enqueue(entry);

        Wake();
    }

    private bool ProcessExpiredEntries() {
        bool didWork = false;

        DateTime now = DateTime.UtcNow;

        foreach (var (channel, retryQueue) in _retryQueues) {
            while (true) {
                var entry = retryQueue.PeekExpired();

                if (entry is not null && now >= entry.ExpiresAt) {
                    retryQueue.DequeueExpired();
                    SettleEntry(entry, now, false, "Notification time to live expires before the notification has been sent");

                    didWork = true;
                } else {
                    break;
                }
            }
        }

        return didWork;
    }

    private bool ProcessRetryEntries(out HashSet<DeliveryChannel> channelsFull) {
        bool didWork = false;

        DateTime now = DateTime.UtcNow;

        channelsFull = new();


        foreach (var (channel, retryQueue) in _retryQueues) {
            

            while (true) {
                var entry = retryQueue.PeekRetryReady();

                if(entry is null || entry.RetryAt!.Value > now) {
                    break;
                }

                // TODO? make this better without dequeue (while still having no race condition)

                retryQueue.DequeueRetryReady();

                bool assigned = TrySubmitEntryToLane(entry);

                if (!assigned) {
                    retryQueue.Enqueue(entry);
                    channelsFull.Add(channel);
                    break;
                }

                didWork = true;
            }
        }

        return didWork;
    }

    private bool TrySubmitEntryToLane(NotificationEntry entry) {
        var channel = entry.Notification.Channel;
        var lanes = _laneStore.FindLanesByChannel(channel);
        var laneIndex = 0;
        bool submitted = false;

        while (laneIndex < lanes.Count) {
            if (!lanes[laneIndex].CanSubmit) {
                laneIndex += 1;
                continue;
            }

            submitted = lanes[laneIndex].TrySubmit(entry);

            if (submitted) {
                break;
            }

            laneIndex += 1;
        }

        return submitted;
    }

    public void SettleEntry(NotificationEntry entry, DateTime settleTime, bool success, string? message = null) {
        using (var _ = entry.Lock()) {
            entry.State = null;

            if (success) {
                entry.Notification.MarkAsSent(settleTime);
            } else {
                entry.Notification.MarkAsFailed(message, settleTime);
            }
            
            _dispatcher.Dispatch(entry.Notification.DomainEvents);
            entry.Notification.ClearDomainEvents();
        }

        _totalCapacityLimiter.Release(1);
    }

    private bool ProcessInboundEntries(HashSet<DeliveryChannel> channelsFull) {
        // no notification retry starving occurs, because if providers are bottlenecked, notifications with past retry times have bigger priority

        bool didWork = false;
        DateTime now = DateTime.UtcNow;

        while (_inbound.Reader.TryRead(out var entry)) {
            var channel = entry.Notification.Channel;
            bool assigned = false;

            if (!channelsFull.Contains(channel)) {
                assigned = TrySubmitEntryToLane(entry);
            }

            if (!assigned) {
                channelsFull.Add(channel);
                var retryQueue = _retryQueues[channel];

                using (var _ = entry.Lock()) {
                    entry.RetryAt = now;
                    entry.State = ProcessingState.QueuedForCapacityRetry;
                }

                retryQueue.Enqueue(entry);
            }

            didWork = true;
        }

        return didWork;
    }

    private TimeSpan FindSleepDuration(HashSet<DeliveryChannel> channelsFull) {
        DateTime minTime = DateTime.MaxValue;

        foreach (var (channel, retryQueue) in _retryQueues) {
            var entry = retryQueue.PeekExpired();

            if (entry is not null) {
                minTime = TimeUtils.Min(minTime, entry.ExpiresAt);
            }

            if (channelsFull.Contains(channel)) {
                // need to skip full channels to prevent busy waiting, because if channel is full (all enabled channel providers are full),
                // it is possible that there are notification entries with past RetryAt date
                continue;
            }

            entry = retryQueue.PeekRetryReady();

            if(entry is not null) {
                minTime = TimeUtils.Min(minTime, entry.RetryAt!.Value);
            }
        }



        TimeSpan waitTime = minTime - DateTime.UtcNow;

        waitTime = TimeUtils.Max(waitTime, TimeSpan.Zero);


        return waitTime;
    }

    private void Sleep(TimeSpan duration) {
        _autoResetEvent.WaitOne(duration);
    }

    public void Wake() {
        _autoResetEvent.Set();
    }

    public void Run(CancellationToken cancellationToken) {
        cancellationToken.Register(Wake);

        while (!cancellationToken.IsCancellationRequested) {
            bool didWork = false;

            HashSet<DeliveryChannel> channelsFull;

            didWork |= ProcessExpiredEntries();
            didWork |= ProcessRetryEntries(out channelsFull);
            didWork |= ProcessInboundEntries(channelsFull);

            if (!didWork) {
                TimeSpan duration = FindSleepDuration(channelsFull);
                if (duration != TimeSpan.Zero) {
                    Sleep(duration);
                }
            }
        }
    }
}
