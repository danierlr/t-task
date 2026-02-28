using NotificationService.Domain.Shared;

namespace NotificationService.Domain.Aggregates.Notifications.Events;

public sealed record NotificationSettledEvent(
    NotificationId NotificationId,
    NotificationStatus Status,
    DateTime SettledAt,
    string? FailReason
) : IDomainEvent { }
