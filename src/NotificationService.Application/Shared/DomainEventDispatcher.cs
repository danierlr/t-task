using NotificationService.Domain.Shared;

namespace NotificationService.Application.Shared;

public class DomainEventDispatcher {
    private readonly Dictionary<Type, List<Action<IDomainEvent>>> _allHandlers = new();

    public void RegisterHandler<TEvent>(IDomainEventHandler<TEvent> handler) where TEvent : IDomainEvent {
        var eventType = typeof(TEvent);

        if (!_allHandlers.ContainsKey(eventType)) {
            _allHandlers[eventType] = new();
        }

        _allHandlers[eventType].Add(domainEvent => handler.Handle((TEvent)domainEvent));
    }

    public void Dispatch(IReadOnlyList<IDomainEvent> domainEvents) {
        foreach (var domainEvent in domainEvents) {
            if (_allHandlers.TryGetValue(domainEvent.GetType(), out var handlers)) {
                foreach (var handler in handlers) {
                    handler(domainEvent);
                }
            }
        }
    }
}
