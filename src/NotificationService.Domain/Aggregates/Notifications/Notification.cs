using NotificationService.Domain.Aggregates.Notifications.Events;
using NotificationService.Domain.Shared;

namespace NotificationService.Domain.Aggregates.Notifications;

public sealed class Notification : Entity {
    public NotificationId Id { get; }

    public NotificationStatus Status { get; private set; }

    public string Message { get; }

    public string? FailReason { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime? SettledAt { get; private set; }

    public Notification(NotificationId id, string message, DateTime createdAt) {
        Id = id;
        Message = message;
        CreatedAt = createdAt;
        Status = NotificationStatus.QueuedForProcessing;
    }

    private void UpdateStatus(NotificationStatus newStatus) {
        Status = newStatus;
    }

    public void MarkAsSent(DateTime sentTime) {
        if (IsSettled()) {
            throw new DomainRuleViolationException($"Cannot transition notification with id:{Id} from {Status} status to {NotificationStatus.Sent} status");
        }

        UpdateStatus(NotificationStatus.Sent);
        SettledAt = sentTime;

        NotificationSettledEvent settledEvent = new(
            NotificationId: Id,
            Status: NotificationStatus.Failed,
            SettledAt: sentTime,
            FailReason: null
        );

        AddEvent(settledEvent);
    }

    public void MarkAsFailed(string? failReason, DateTime failTime) {
        if (IsSettled()) {
            throw new DomainRuleViolationException($"Cannot transition notification with id:{Id} from {Status} status to {NotificationStatus.Failed} status");
        }

        UpdateStatus(NotificationStatus.Failed);
        FailReason = failReason;
        SettledAt = failTime;

        NotificationSettledEvent settledEvent = new(
            NotificationId: Id,
            Status: NotificationStatus.Failed,
            SettledAt: failTime,
            FailReason: failReason
        );

        AddEvent(settledEvent);
    }

    public bool IsSettled() {
        return Status == NotificationStatus.Sent || Status == NotificationStatus.Failed;
    }
}
