using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Notifications;
using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Api.Notifications;

[ApiController]
[Route("api/v1/notification")]
public class NotificationController : ControllerBase {
    private readonly INotificationSender _notificationSender;
    public NotificationController(INotificationSender notificationSender) {
        _notificationSender = notificationSender;
    }

    [HttpPost]
    public async Task<IActionResult> SendSingle([FromBody] SendNotificationHttpRequest httpRequest, CancellationToken cancellationToken) {
        var request = httpRequest.ToSendNotificationRequest();

        var result = await _notificationSender.SendAsync(request, cancellationToken);

        SendNotificationHttpResponse response = new() {
            NotificationId = result.NotificationId.ToString(),
            Channel = request.Channel.ToString(),
            Status = result.Status.ToString(),
            UsedProviderName = result.UsedProviderName,
            SettledAt = result.SettledAt?.ToString(),
            CreatedAt = result.CreatedAt.ToString(),
            RetryCount = result.RetryCount,
        };


        if (result.Status == NotificationStatus.Processing) {
            return Accepted(response);
        }

        return Ok(response);
    }
}
