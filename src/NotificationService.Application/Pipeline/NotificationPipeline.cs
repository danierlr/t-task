using NotificationService.Application.Utils;
using NotificationService.Domain.Aggregates.Notifications;
using System.Threading.Channels;

namespace NotificationService.Application.Pipeline;

public class NotificationPipeline {
    private readonly CapacityLimiter _globalCapacityLimiter = new CapacityLimiter(10000);

    private readonly Channel<Notification> _inbound;

    public NotificationPipeline() {
        _inbound = Channel.CreateUnbounded<Notification>();
    }

    public bool TrySubmit(Notification notification) {
        bool reserved = _globalCapacityLimiter.TryReserve(1);

        if (!reserved) {
            return false;
        }

        bool written = _inbound.Writer.TryWrite(notification);

        if (!written) {
            _globalCapacityLimiter.Release(1);
            throw new InvalidOperationException("Unbound channel rejected write");
        }

        return true;
    }



    public void Run(CancellationToken cancellationToken) { 
    }
}
