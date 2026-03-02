using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Notifications.Dtos;

public sealed record SendNotificationRequest(
    Recipient Recipient,
    string Message,
    DeliveryChannel Channel
);
