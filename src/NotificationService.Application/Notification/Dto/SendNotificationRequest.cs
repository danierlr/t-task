using System;
using System.Collections.Generic;
using System.Text;
using NotificationService.Domain.Aggregates.Notification;

namespace NotificationService.Application.Notification.Dto;

public sealed record SendNotificationRequest(
    Recipient Recipient,
    string Message,
    Channel Channel
);
