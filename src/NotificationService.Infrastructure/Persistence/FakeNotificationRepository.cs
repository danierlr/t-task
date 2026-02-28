using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Ports;

namespace NotificationService.Infrastructure.Persistence;

internal class FakeNotificationRepository : INotificationRepository {
    public Task CreateAsync(Notification notification) {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Notification notification) {
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<Notification> GetAllNotSettled() {
        yield break;
    }
}
