using NotificationService.Domain.Aggregates.Notification;

namespace NotificationService.Application.Notifications.Dtos;

public sealed record SendNotificationRequest(
    Recipient Recipient,
    string Message,
    Channel Channel
);
