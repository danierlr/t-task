using NotificationService.Application.Notifications;

namespace NotificationService.Application.RetryQueue;

// Enqueue/Dequeue functions are sync, because the pipeline is optimized for in-memory storage overall
// For a message broker based storage it would be better to have a different pipeline architecture completely

public interface IRetryQueue {
    public NotificationEntry? DequeueExpired();

    public NotificationEntry? PeekExpired();

    public NotificationEntry? DequeueRetryReady();

    public NotificationEntry? PeekRetryReady();

    public void Enqueue(NotificationEntry entry);
}
