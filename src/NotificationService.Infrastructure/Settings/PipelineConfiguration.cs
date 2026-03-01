using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Infrastructure.Settings;

public class PipelineConfiguration {
    public long TotalNotificationCapacity { get; set; }

    public long ConfigurationUpdateTimeoutMs { get; set; }

    public Dictionary<DeliveryChannel, ChannelConfiguration> Channels { get; set; } = new();
}
