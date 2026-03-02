using NotificationService.Application.Notifications.Dtos;
using NotificationService.Application.Settings;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Notifications;

public interface INotificationSender: IReconfigurable {
    Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken);
}
