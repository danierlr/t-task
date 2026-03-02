namespace NotificationService.Api.Notifications; 
public sealed class SendNotificationHttpResponse {
    public string NotificationId { get; set; } = string.Empty;

    public string? Channel { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? UsedProviderName { get; set; } = string.Empty;

    public string? SettledAt { get; set; }

    public string CreatedAt { get; set; } = string.Empty;

    public long RetryCount { get; set; }
}
