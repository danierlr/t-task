using NotificationService.Application.Notifications.Dtos;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Notifications;

public interface INotificationSender {
    Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken);
}
