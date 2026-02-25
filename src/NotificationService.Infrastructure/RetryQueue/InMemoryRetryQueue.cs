using NotificationService.Application.RetryQueue;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Infrastructure.RetryQueue;

internal class InMemorySimpleRetryQueue : IRetryQueue {
    private Dictionary<NotificationId, Notification> _notifications = new();
    private MinPriorityQueueRemovable<NotificationId, DateTime> _retry = new();
    private MinPriorityQueueRemovable<NotificationId, DateTime> _timeout = new();


    public IReadOnlyList<Notification> DequeueRetryReady(DateTime time, long maxCount)
        => Dequeue(_retry, _timeout, time, maxCount);


    public IReadOnlyList<Notification> DequeueTimedOut(DateTime time, long maxCount)
        => Dequeue(_timeout, _retry, time, maxCount);

    private IReadOnlyList<Notification> Dequeue(
        MinPriorityQueueRemovable<NotificationId, DateTime> primary,
        MinPriorityQueueRemovable<NotificationId, DateTime> secondary,
        DateTime time,
        long maxCount
    ) {
        List<Notification> results = new();

        while (results.Count < maxCount) {
            bool found = primary.TryDequeueMin(out NotificationId notificationId, time);

            Notification? notification = null;

            if (found) {
                notification = _notifications[notificationId];
                _notifications.Remove(notificationId);
                secondary.Remove(notificationId);
                results.Add(notification);
            } else {
                break;
            }
        }

        return results;
    }

    public void Enqueue(Notification notification, DateTime ready, DateTime timeout) {
        if (_notifications.TryGetValue(notification.Id, out var _)) {
            throw new InvalidOperationException($"Notification with id {notification.Id.Value} is present in queue already");
        }

        _notifications[notification.Id] = notification;
        _retry.Enqueue(notification.Id, ready);
        _timeout.Enqueue(notification.Id, timeout);
    }
}
