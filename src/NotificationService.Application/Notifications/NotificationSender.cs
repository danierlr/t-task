using NotificationService.Application.Notifications.Dtos;
using NotificationService.Application.Utils;

namespace NotificationService.Application.Notifications; 
public class NotificationSender : INotificationSender {
    
    public Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken) {

    }

    public Task<SendNotificationResult> RetrySendAsync(string notificationId, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}s
