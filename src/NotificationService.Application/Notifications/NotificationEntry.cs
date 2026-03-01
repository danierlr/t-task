using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Notifications;

public class NotificationEntry : ILockable {
    public Notification Notification { get; }

    public ProcessingState? State { get; set; } = ProcessingState.QueuedForProcessing;

    public SyncRoot SyncRoot { get; } = new();

    public int RetryCount { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RetryAt { get; set; }

    public NotificationEntry(Notification notification, DateTime expiresAt) {
        Notification = notification;
        ExpiresAt = expiresAt;
    }
}
