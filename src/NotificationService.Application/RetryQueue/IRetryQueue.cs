using NotificationService.Application.Notifications;

namespace NotificationService.Application.RetryQueue;

// Enqueue/Dequeue functions are sync, because the pipeline is optimized for in-memory storage overall
// For a message broker based storage it would be better to have a different pipeline architecture completely

public interface IRetryQueue {
    public IReadOnlyList<NotificationEntry> DequeueExpired(DateTime maxExpiresAt, long maxCount);

    public NotificationEntry? PeekExpired();

    public IReadOnlyList<NotificationEntry> DequeueRetryReady(DateTime maxReadyAt, long maxCount);

    public NotificationEntry? PeekRetryReady();

    public void Enqueue(NotificationEntry entry);
}
