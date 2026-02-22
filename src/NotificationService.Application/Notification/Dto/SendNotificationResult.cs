using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Notification.Dto;

public sealed record SendNotificationResult(
    string NotificationId
);
