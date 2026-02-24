using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Aggregates.Notifications;

public enum DeliveryChannel {
    Sms,
    Email,
    Push
}
