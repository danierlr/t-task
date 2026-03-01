namespace NotificationService.Domain.Aggregates.Notifications;

public enum NotificationStatus {
    /// <summary>
    /// Notification is being processed in the pipeline
    /// </summary>
    Processing,

    /// <summary>
    /// Sent successfully
    /// </summary>
    Sent,

    /// <summary>
    /// Failed permanently
    /// </summary>
    Failed,
}
