using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Aggregates.Notifications;

public class Notification {
    public void MarkAsSent() {

    }

    public void MarkAsFailed() {

    }

    public bool isQueued() {
        throw new Exception("not implemented");
    }
}
