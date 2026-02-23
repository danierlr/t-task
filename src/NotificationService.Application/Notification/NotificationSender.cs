using NotificationService.Application.Notification.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Notification; 
public class NotificationSender : INotificationSender {
    public Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken) {

        request
        throw new NotImplementedException();
    }

    public Task<SendNotificationResult> RetrySendAsync(string notificationId, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
