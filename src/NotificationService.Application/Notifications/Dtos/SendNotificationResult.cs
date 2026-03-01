using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Notifications.Dtos;

public sealed record SendNotificationResult(
    NotificationId NotificationId,
    NotificationStatus Status,
    DateTime CreatedAt,
    DateTime? SettledAt,
    string? ErrorMessage
);
