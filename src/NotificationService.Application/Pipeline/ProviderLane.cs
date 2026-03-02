using NotificationService.Application.Notifications;
using NotificationService.Application.Settings;
using NotificationService.Application.Shared;
using NotificationService.Domain.Ports;
using System.Threading.Channels;

// TODO add cancellation grace period for the started external requests

namespace NotificationService.Application.Pipeline {
    public class ProviderLane {
        private readonly CapacityLimiter _capacityLimiter;

        private readonly CapacityLimiter _concurrencyLimiter;

        private readonly Channel<NotificationEntry> _inbound;

        private readonly CancellationToken _cancellationToken;

        private readonly NotificationPipeline _notificationPipeline;

        private volatile ProviderLaneSettings _settings;

        public INotificationProvider Provider { get; }

        public ProviderLane(INotificationProvider provider, ProviderLaneSettings settings, NotificationPipeline notificationPipeline, CancellationToken cancellationToken) {
            _cancellationToken = cancellationToken;
            _notificationPipeline = notificationPipeline;
            _concurrencyLimiter = new(settings.NumConcurrencySlots);
            _capacityLimiter = new(settings.BufferCapacity);
            _settings = settings;

            Provider = provider;

            _inbound = Channel.CreateUnbounded<NotificationEntry>(new UnboundedChannelOptions {
                SingleWriter = true,
                SingleReader = false,
            });
        }

        public ProviderLaneSettings Settings => _settings;

        public bool TrySubmit(NotificationEntry notificationEntry) {
            var reserved = _capacityLimiter.TryReserve(1);

            if (!reserved) {
                return false;
            }

            using (var _ = notificationEntry.Lock()) {
                bool written = _inbound.Writer.TryWrite(notificationEntry);

                if (!written) {
                    _capacityLimiter.Release(1);
                    throw new InvalidOperationException("Could not write to unbound channel");
                }

                notificationEntry.State = ProcessingState.QueuedForProvider;
            }

            TrySpawnWorker();
            return true;
        }

        private void TrySpawnWorker() {
            bool reserved = _concurrencyLimiter.TryReserve(1);

            if (reserved) {
                Task.Run(RunWorker);
            }
        }

        private async Task RunWorker() {
            while (_inbound.Reader.TryRead(out var entry)) {
                _capacityLimiter.Release(1);

                _notificationPipeline.Wake();

                using (var _ = entry.Lock()) {
                    entry.State = ProcessingState.Sending;
                }

                if (_settings.Provider.Enabled) {
                    try {
                        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);

                        timeoutCts.CancelAfter(_settings.SendTimeout);

                        bool sent = await Provider.TrySendAsync(entry.Notification.Recipient, entry.Notification.Message, timeoutCts.Token);

                        if (sent) {
                            _notificationPipeline.SettleEntry(entry, DateTime.UtcNow, true);
                        } else {
                            _notificationPipeline.SubmitRetry(entry);
                        }
                    } catch {
                        _notificationPipeline.SubmitRetry(entry);
                    }
                } else {
                    _notificationPipeline.SubmitRetry(entry);
                }

                // need to release and reserve to adjust for concurrency slot count changes
                _concurrencyLimiter.Release(1);
                bool reserved = _concurrencyLimiter.TryReserve(1);

                if (!reserved) {
                    return;
                }
            }

            _concurrencyLimiter.Release(1);

            // prevents stranded concurrency slot if TrySpawnWorker occurs after TryRead and before _concurrencyLimiter.Release
            if (_inbound.Reader.TryPeek(out _)) {
                TrySpawnWorker();
            }
        }

        public void ApplySettings(ProviderLaneSettings settings) {


            if (settings.NumConcurrencySlots != _concurrencyLimiter.Capacity) {
                _concurrencyLimiter.SetCapacity(settings.NumConcurrencySlots);
            }

            if (settings.BufferCapacity != _capacityLimiter.Capacity) {
                _capacityLimiter.SetCapacity(settings.BufferCapacity);
            }

            _settings = settings;
        }

        public bool CanSubmit => _capacityLimiter.Count < _capacityLimiter.Capacity && _settings.Provider.Enabled;
    }
}
