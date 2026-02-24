using NotificationService.Application.Notifications.Dtos;

namespace NotificationService.Application.Notifications;

public interface INotificationSender {
    Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken);
    Task<SendNotificationResult> RetrySendAsync(string notificationId, CancellationToken cancellationToken);
}
