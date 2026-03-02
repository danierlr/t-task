using NotificationService.Application.Notifications;
using NotificationService.Application.Pipeline;
using NotificationService.Application.Shared;
using NotificationService.Domain.Ports;

namespace NotificationService.Application.Initialization;

public class InitializationService {
    private readonly DomainEventDispatcher _domainEventDispatcher;
    private readonly NotificationSettledHandler _notificationSettledHandler;
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationPipeline _notificationPipeline;

    public InitializationService(
        DomainEventDispatcher domainEventDispatcher,
        NotificationSettledHandler notificationSettledHandler,
        INotificationRepository notificationRepository,
        NotificationPipeline notificationPipeline
    ) {
        _domainEventDispatcher = domainEventDispatcher;
        _notificationSettledHandler = notificationSettledHandler;
        _notificationRepository = notificationRepository;
        _notificationPipeline = notificationPipeline;
    }

    public async Task Initialize(CancellationToken cancellationToken) {
        _domainEventDispatcher.RegisterHandler(_notificationSettledHandler);

        await foreach (var notification in _notificationRepository.GetAllNotSettled()) {
            // TODO submit unfinished notifications into the notification pipeline
        }
    }
}
