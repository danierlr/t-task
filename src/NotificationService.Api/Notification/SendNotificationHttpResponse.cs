namespace NotificationService.Api.Notification; 
public sealed class SendNotificationHttpResponse {
    public string NotificationId { get; set; } = string.Empty;

    public string? Channel { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ProviderUsed { get; set; } = string.Empty;

    public string? SentAt { get; set; }

    public string CreatedAt { get; set; } = string.Empty;

    public int RetryCount { get; set; }
}
