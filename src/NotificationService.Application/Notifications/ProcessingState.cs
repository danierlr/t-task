using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Notifications;

public enum ProcessingState {
    /// <summary>
    /// New notification waiting to be handled by the background worker
    /// </summary>
    QueuedForProcessing,

    /// <summary>
    /// Waiting in the provider buffer (assigned to concrete provider for processing)
    /// </summary>
    QueuedForProvider,

    /// <summary>
    /// Is being processed by the provider (request sent to the external provider)
    /// </summary>
    Sending,

    /// <summary>
    /// Waiting in global retry queue, due to all relevant providers busy
    /// </summary>
    QueuedForCapacityRetry,

    /// <summary>
    /// Waiting in global retry queue, due to provider delivery fail
    /// </summary>
    QueuedForProviderRetry,
}
