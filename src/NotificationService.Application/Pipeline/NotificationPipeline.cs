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

        if(entry.RetryCount >= channelSettings.MaxNumRetries) {
            using (var _ = entry.Lock()) {
                entry.State = null;
                entry.RetryAt = null;

                entry.Notification.MarkAsFailed($"Exceeded maximum retry limit of {channelSettings.MaxNumRetries} attempts", now);
            }
        }

        DateTime retryAt = RetryPolicy.FindRetryTime(
            entry.RetryCount, now,
            channelSettings.BackoffMultiplier,
            channelSettings.InitialRetryDelay,
            channelSettings.MaxRetryDelay
        );
        
        using (var _ = entry.Lock()) {
            entry.State = ProcessingState.QueuedForProviderRetry;
            entry.RetryCount += 1;
            entry.RetryAt = retryAt;
        }
    }

    private bool ProcessExpiredEntries() {
        // TODO? batch dequeue

        bool didWork = false;

        DateTime now = DateTime.UtcNow;

        foreach (var (channel, retryQueue) in _retryQueues) {
            NotificationEntry? timedOutEntry = null;

            while (true) {
                var timedOutList = retryQueue.DequeueExpired(now, 1);
                timedOutEntry = timedOutList.Count > 0 ? timedOutList[0] : null;

                if (timedOutEntry is not null) {
                    using (var _ = timedOutEntry.Lock()) {
                        timedOutEntry.State = null;
                        timedOutEntry.Notification.MarkAsFailed("Notification time to live expired before being sent", now);
                        _dispatcher.Dispatch(timedOutEntry.Notification.DomainEvents);
                        timedOutEntry.Notification.ClearDomainEvents();
                    }

                    _totalCapacityLimiter.Release(1);

                    didWork = true;
                } else {
                    break;
                }
            }
        }

        return didWork;
    }

    private bool ProcessRetryEntries() {
        // TODO this can be optimized a bit

        bool didWork = false;

        DateTime now = DateTime.UtcNow;

        foreach (var (channel, retryQueue) in _retryQueues) {
            var lanes = _laneStore.FindLanesByChannel(channel);
            var laneIndex = 0;

            while (laneIndex < lanes.Count) {
                if (!lanes[laneIndex].CanSubmit) {
                    laneIndex += 1;
                    continue;
                }

                var retryEntryList = retryQueue.DequeueRetryReady(now, 1);

                var retryEntry = retryEntryList.Count > 0 ? retryEntryList[0] : null;

                if (retryEntry is null) {
                    break;
                }

                var submitted = lanes[laneIndex].TrySubmit(retryEntry);

                if (!submitted) {
                    retryQueue.Enqueue(retryEntry);
                    laneIndex += 1;
                    continue;
                }

                didWork = true;
            }
        }

        return didWork;
    }

    private bool ProcessInboxEntries() {
        throw new NotImplementedException();
    }

    private TimeSpan FindSleepDuration() {
        throw new NotImplementedException();
        //return Timeout.InfiniteTimeSpan;

    }

    private void Sleep(TimeSpan duration) {
        _autoResetEvent.WaitOne(duration);
    }

    private void Wake() {
        _autoResetEvent.Set();
    }

    public void Run(CancellationToken cancellationToken) {
        cancellationToken.Register(Wake);

        while (!cancellationToken.IsCancellationRequested) {
            bool didWork = false;

            didWork |= ProcessExpiredEntries();
            didWork |= ProcessRetryEntries();
            didWork |= ProcessInboxEntries();

            if (!didWork) {
                TimeSpan duration = FindSleepDuration();
                Sleep(duration);
            }
        }
    }
}
