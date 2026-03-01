using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Ports;
using System.Threading.Channels;

public class Foo: ILockable {
    public void Bar() {

    }
}

namespace NotificationService.Application.Pipeline {
    public class ProviderLane {
        private readonly CapacityLimiter _capacityLimiter;

        private readonly int _numConcurrencySlots;

        private readonly INotificationProvider _provider;

        private readonly Channel<Notification> _inbound;

        public ProviderLane(INotificationProvider provider, int initialNumConcurrencySlots, long initialCapacity) {
            _provider = provider;
            _numConcurrencySlots = initialNumConcurrencySlots;
            _capacityLimiter = new(initialCapacity);

            _inbound = Channel.CreateUnbounded<Notification>(new UnboundedChannelOptions {
                SingleWriter = true,
                SingleReader = false,
            });
        }

        //public GetAvailableCapacity() {} // 

        public bool TrySubmit(Notification notification) {
            var reserved = _capacityLimiter.TryReserve(1);

            if(!reserved) {
                return false;
            }

            bool written = _inbound.Writer.TryWrite(notification);

            if ((!written)) {
                _capacityLimiter.Release(1);
                throw new InvalidOperationException("Could not write to unbound channel");
            }

        }
    }
}
