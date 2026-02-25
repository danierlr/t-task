using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Notifications;

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

        var response = await _notificationSender.SendAsync(request, cancellationToken);

        return Ok("test");
    }
}
