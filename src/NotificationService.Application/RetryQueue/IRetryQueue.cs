using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.RetryQueue;

// Enqueue/Dequeue functions are sync, because the pipeline is optimized for in-memory storage overall
// For a message broker based storage it would be better to have a different pipeline architecture completely

public interface IRetryQueue {
    public IReadOnlyList<Notification> DequeueTimedOut(DateTime time, long maxCount);

    public IReadOnlyList<Notification> DequeueRetryReady(DateTime time, long maxCount);

    public void Enqueue(Notification notification, DateTime ready, DateTime timeout);
}
