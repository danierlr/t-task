using NotificationService.Application.Notifications.Dtos;
using NotificationService.Application.Pipeline;
using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Ports;

namespace NotificationService.Application.Notifications;

public class NotificationSender : INotificationSender {
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationPipeline _notificationPipeline;
    private readonly IIdGenerator _idGenerator;
    private readonly NotificationSettleRegistry _notificationSettleRegistry;

    public NotificationSender(INotificationRepository notificationRepository, NotificationPipeline notificationPipeline, IIdGenerator idGenerator, NotificationSettleRegistry notificationSettleRegistry) {
        _notificationRepository = notificationRepository;
        _notificationPipeline = notificationPipeline;
        _idGenerator = idGenerator;
        _notificationSettleRegistry = notificationSettleRegistry;
    }

    public async Task<SendNotificationResult> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken) {
        var id = NotificationId.New(_idGenerator);

        DateTime currentTime = DateTime.UtcNow;
        DateTime lingerDeadline = currentTime + TimeSpan.FromMilliseconds(2000);

        var notification = new Notification(id, request.Message, currentTime);

        await _notificationRepository.CreateAsync(notification);

        SendNotificationResult result = new SendNotificationResult(
            NotificationId: id,
            Status: NotificationStatus.QueuedForProcessing,
            CreatedAt: currentTime,
            SettledAt: null,
            ErrorMessage: null
        );

        bool submitted = _notificationPipeline.TrySubmit(notification);

        if (!submitted) {
            DateTime failTime = DateTime.UtcNow;
            string failReason = "Notification pipeline is at a full capacity, can not submit new notification";

            notification.MarkAsFailed(failReason, failTime);

            await _notificationRepository.UpdateAsync(notification);

            result = result with {
                Status = NotificationStatus.Failed,
                ErrorMessage = failReason,
                SettledAt = failTime,
            };

            return result;
        }

        var completionEvent = await _notificationSettleRegistry.WaitForSettle(id, lingerDeadline);

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