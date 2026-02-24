using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Domain.Ports;

public abstract class NotificationProvider<TRecipient> : INotificationProvider {
    public abstract string Name { get; }

    public abstract DeliveryChannel Channel { get; }

    public Task<bool> TrySendAsync(Recipient recipient, string message, CancellationToken cancellationToken) {
        if (recipient is not TRecipient typedRecipient) {
            throw new ArgumentException($"Wrong recipient type. Expected type: {typeof(TRecipient).Name}");
        }

        return TryInternalSendAsync(typedRecipient, message, cancellationToken);
    }

    protected abstract Task<bool> TryInternalSendAsync(TRecipient recipient, string message, CancellationToken cancellationToken);
}
