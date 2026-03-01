using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Notifications;

public class NotificationEntry: ILockable {
    public Notification Notification { get; }

    public SyncRoot SyncRoot { get; } = new();

    public NotificationEntry(Notification notification) {
        Notification = notification;
        ProcessingState = ProcessingState.QueuedForProcessing;
    }

    public ProcessingState ProcessingState { get; set; }
}
