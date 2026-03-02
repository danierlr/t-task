using NotificationService.Application.Notifications;
using NotificationService.Application.RetryQueue;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Infrastructure.RetryQueue;

internal class InMemorySimpleRetryQueue : IRetryQueue {
    private Dictionary<NotificationId, NotificationEntry> _entries = new();
    private MinPriorityQueueRemovable<NotificationId, DateTime> _retry = new();
    private MinPriorityQueueRemovable<NotificationId, DateTime> _expired = new();


    public NotificationEntry? DequeueRetryReady()
        => Dequeue(_retry, _expired);

    public NotificationEntry? DequeueExpired()
        => Dequeue(_expired, _retry);

    public NotificationEntry? PeekRetryReady() => Peek(_retry);

    public NotificationEntry? PeekExpired() => Peek(_expired);

    private NotificationEntry? Dequeue(
        MinPriorityQueueRemovable<NotificationId, DateTime> primary,
        MinPriorityQueueRemovable<NotificationId, DateTime> secondary
    ) {
        bool found = primary.TryDequeue(out NotificationId notificationId);
        if (!found) {
            return null;
        }

        NotificationEntry entry = _entries[notificationId];

        _entries.Remove(notificationId);
        secondary.Remove(notificationId);

        return entry;
    }

    private NotificationEntry? Peek(MinPriorityQueueRemovable<NotificationId, DateTime> queue) {
        bool exists = queue.TryPeek(out NotificationId notificationId);

        if (exists) {
            return _entries[notificationId];
        }

        return null;
    }

    public void Enqueue(NotificationEntry entry) {
        if (_entries.TryGetValue(entry.Notification.Id, out var _)) {
            throw new InvalidOperationException($"Notification with id {entry.Notification.Id} is present in queue already");
        }

        if (entry.RetryAt is null) {
            throw new InvalidOperationException($"Can not enqueue notification with id {entry.Notification.Id} RetryAt is not specified");
        }

        _entries[entry.Notification.Id] = entry;
        _retry.Enqueue(entry.Notification.Id, entry.RetryAt.Value);
        _expired.Enqueue(entry.Notification.Id, entry.ExpiresAt);
    }
}
