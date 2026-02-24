namespace NotificationService.Domain.Aggregates.Notifications;

public enum NotificationStatus {
    // new notification waiting to be handled by the background worker
    QueuedForProcessing,

    // waiting in the provider buffer (assigned to concrete provider for processing)
    QueuedForProvider,

    // is being processed by the provider (request sent to the external provider)
    Sending,

    // sent successfully
    Sent,

    // waiting in global retry queue, due to all relevant providers busy
    QueuedForCapacityRetry,

    // waiting in global retry queue, due to provider delivery fail
    QueuedForProviderRetry,
    
    // failed permanently
    Failed,
}
