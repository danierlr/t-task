using NotificationService.Application.Notification.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Notification;

public interface INotificationSender {
    Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken);
    Task<SendNotificationResult> RetrySendAsync(string notificationId, CancellationToken cancellationToken);
}
