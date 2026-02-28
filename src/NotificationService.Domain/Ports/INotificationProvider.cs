using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Domain.Ports;

public interface INotificationProvider {
    string Name { get; }

    DeliveryChannel Channel { get; }

    Task<bool> TrySendAsync(Recipient recipient, string message, CancellationToken cancellationToken);
}
