using NotificationService.Domain.Aggregates.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Notifications;

public class LockToken : IDisposable {

    public void Dispose() {
        throw new NotImplementedException();
    }
}

public class NotificationContext {
    public Notification Notification { get; }

    public NotificationContext(Notification notification) {
        Notification = notification;
        ProcessingState = ProcessingState.QueuedForProcessing;
    }

    public ProcessingState ProcessingState { get; set; }
}
