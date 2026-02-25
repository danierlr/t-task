namespace NotificationService.Domain.Aggregates.Notifications;

public class Notification {
    public NotificationId Id { get; }

    public Notification(NotificationId id) {
        Id = id;
    }

    public void MarkAsSent() {
        throw new NotImplementedException();
    }

    public void MarkAsFailed() {
        throw new NotImplementedException();
    }

    public bool IsQueued() {
        throw new NotImplementedException();
    }
}
