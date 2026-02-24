using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Notification; 


// TODO add batch notifications
// TODO? maybe use domain value objects and simple types directly
public sealed class SendNotificationHttpRequest {
    [Required]
    public Dictionary<string, string> Recipient { get; set; } = new();

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public string Channel { get; set; } = string.Empty;

    public string? CallbackUrl { get; set; }
}
