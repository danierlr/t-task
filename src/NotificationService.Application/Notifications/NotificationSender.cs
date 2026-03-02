using NotificationService.Application.Notifications.Dtos;
using NotificationService.Application.Pipeline;
using NotificationService.Application.Settings;
using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Ports;

namespace NotificationService.Application.Notifications;

public class NotificationSender : INotificationSender {
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationPipeline _notificationPipeline;
    private readonly IIdGenerator _idGenerator;
    private readonly NotificationSettleRegistry _notificationSettleRegistry;
    private readonly DomainEventDispatcher _dispatcher;
    private PipelineSettings _settings;

    public NotificationSender(
        INotificationRepository notificationRepository,
        NotificationPipeline notificationPipeline,
        IIdGenerator idGenerator,
        NotificationSettleRegistry notificationSettleRegistry,
        DomainEventDispatcher dispatcher,
        PipelineSettings initialSettings
    ) {
        _settings = initialSettings;
        _notificationRepository = notificationRepository;
        _notificationPipeline = notificationPipeline;
        _idGenerator = idGenerator;
        _dispatcher = dispatcher;
        _notificationSettleRegistry = notificationSettleRegistry;
    }

    public void ApplySettings(PipelineSettings settings) {
        _settings = settings;
    }

    public async Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken) {
        var id = NotificationId.New(_idGenerator);
        var channelSettings = _settings.Channels[request.Channel];

        DateTime currentTime = DateTime.UtcNow;
        DateTime lingerUntil = currentTime + channelSettings.ResultLingerDuration;
        DateTime expiresAt = currentTime + channelSettings.TimeToLive;


        Notification notification = new(
            id: id,
            deliveryChannel: request.Channel,
            message: request.Message,
            recipient: request.Recipient,
            createdAt: currentTime
        );

        NotificationEntry notificationEntry = new(notification, expiresAt);

        await _notificationRepository.CreateAsync(notification);

        SendNotificationResult result = new SendNotificationResult(
            NotificationId: id,
            Status: NotificationStatus.Processing,
            CreatedAt: currentTime,
            SettledAt: null,
            ErrorMessage: null
        );

        bool submitted = _notificationPipeline.TrySubmitNew(notificationEntry);

        if (!submitted) {
            DateTime failTime = DateTime.UtcNow;
            string failReason = "Notification pipeline is at a full capacity, can not submit new notification";

            notification.MarkAsFailed(failReason, failTime);
            _dispatcher.Dispatch(notification.DomainEvents);
            notification.ClearDomainEvents();
            notificationEntry.State = null;

            await _notificationRepository.UpdateAsync(notification);

            result = result with {
                Status = NotificationStatus.Failed,
                ErrorMessage = failReason,
                SettledAt = failTime,
            };

            return result;
        }

        var completionEvent = await _notificationSettleRegistry.WaitForSettle(id, lingerUntil);

        if (completionEvent is null) {
            return result; // TODO add events for each status transition and return proper proper status
        }

        result = result with {
            Status = completionEvent.Status,
            SettledAt = completionEvent.SettledAt,
            ErrorMessage = completionEvent.FailReason,
        };

        return result;
    }
}
