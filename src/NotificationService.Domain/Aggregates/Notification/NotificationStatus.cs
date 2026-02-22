using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Aggregates.Notification; 
public enum NotificationStatus {
    Sent,
    QueuedForRetry,
    Failed,
}
