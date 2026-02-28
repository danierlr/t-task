using NotificationService.Domain.Shared;

namespace NotificationService.Application.Shared;

public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent {
    void Handle(TEvent domainEvent);
}
