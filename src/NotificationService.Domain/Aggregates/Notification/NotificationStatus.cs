using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Aggregates.Notification; 
public enum NotificationStatus {
    // sent successfully
    Sent,
    // waiting in global retry queue
    QueuedForRetry,
    // waiting in the provider buffer
    QueuedForProvider,
    // is being processed by the provider (request sent to the external provider)
    Sending,
    // failed permanently
    Failed,
}
