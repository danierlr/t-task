using NotificationService.Application.Notifications;
using NotificationService.Application.Shared;
using NotificationService.Domain.Ports;
using System.Threading.Channels;

namespace NotificationService.Application.Pipeline {
    public class ProviderLane {
        private readonly CapacityLimiter _capacityLimiter;

        private readonly int _numConcurrencySlots;

        private readonly INotificationProvider _provider;

        private readonly Channel<NotificationEntry> _inbound;

        public ProviderLane(INotificationProvider provider, int initialNumConcurrencySlots, long initialCapacity) {
            _provider = provider;
            _numConcurrencySlots = initialNumConcurrencySlots;
            _capacityLimiter = new(initialCapacity);

            _inbound = Channel.CreateUnbounded<NotificationEntry>(new UnboundedChannelOptions {
                SingleWriter = true,
                SingleReader = false,
            });
        }

        public bool TrySubmit(NotificationEntry notificationEntry) {
            var reserved = _capacityLimiter.TryReserve(1);

            if(!reserved) {
                return false;
            }

            using (var _ = notificationEntry.Lock()) {
                bool written = _inbound.Writer.TryWrite(notificationEntry);

                if (!written) {
                    _capacityLimiter.Release(1);
                    throw new InvalidOperationException("Could not write to unbound channel");
                }

                notificationEntry.ProcessingState = ProcessingState.QueuedForProvider;
            }

            return true;
        }

        public bool CanSubmit => _capacityLimiter.Count < _capacityLimiter.Capacity;
    }
}
