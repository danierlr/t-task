using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Aggregates.Notifications.Events;
using System.Collections.Concurrent;

namespace NotificationService.Application.Notifications;

public class NotificationSettleRegistry {
    private ConcurrentDictionary<NotificationId, TaskCompletionSource<NotificationSettledEvent>> _completions = new();

    public async Task<NotificationSettledEvent?> WaitForSettle(NotificationId notificationId, DateTime waitDeadline) {
        TaskCompletionSource<NotificationSettledEvent> completion = new();

        _completions[notificationId] = completion;

        DateTime timeNow = DateTime.UtcNow;

        if(timeNow >= waitDeadline) {
            return null;
        }

        TimeSpan deadlineDelay = waitDeadline - timeNow;

        var result = await completion.Task.WithTimeout(deadlineDelay);

        _completions.TryRemove(notificationId, out var _);

        if (result.Completed) {
            return result.Value;
        }

        return null;
    }

    public void Settle(NotificationSettledEvent settledEvent) {
        var exists = _completions.TryRemove(settledEvent.NotificationId, out var completion);

        if(!exists || completion is null) {
            return;
        }

        completion.TrySetResult(settledEvent);
    }
}
