using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications.Events;

namespace NotificationService.Application.Notifications;

public class NotificationSettledHandler : IDomainEventHandler<NotificationSettledEvent> {
    private readonly NotificationSettleRegistry _notificationSettleRegistry;

    public NotificationSettledHandler(NotificationSettleRegistry notificationSettleRegistry) {
        _notificationSettleRegistry = notificationSettleRegistry;
    }

    public void Handle(NotificationSettledEvent domainEvent) {
        _notificationSettleRegistry.Settle(domainEvent);
    }
}
