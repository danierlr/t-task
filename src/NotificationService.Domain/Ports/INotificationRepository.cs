using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Domain.Ports;

public interface INotificationRepository {
    Task CreateAsync(Notification notification);
    
    Task UpdateAsync(Notification notification);

    IAsyncEnumerable<Notification> GetAllNotSettled();
}
