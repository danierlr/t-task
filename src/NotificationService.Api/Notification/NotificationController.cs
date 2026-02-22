using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Api.Notification {
    [ApiController]
    [Route("api/v1/notification")]
    public class NotificationController : ControllerBase {
        public NotificationController() {

        }

        [HttpPost]
        public async Task<IActionResult> SendSingle([FromBody] SendNotificationHttpRequest request, CancellationToken cancellationToken) {

            return Ok("test");
        }
    }
}
