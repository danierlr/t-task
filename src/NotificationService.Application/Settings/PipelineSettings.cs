using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Settings;

public record PipelineSettings {
    /// <summary>
    /// How many notifications can be queued in total
    /// </summary>
    public long TotalNotificationCapacity { get; init; }

    // TODO fix the name later
    /// <summary>
    /// How long to wait for the pipeline worker to pause
    /// </summary>
    public TimeSpan ConfigurationUpdateTimeout { get; init; }

    public IReadOnlyDictionary<(DeliveryChannel Channel, string ProviderName), ProviderLaneSettings> Lanes { get; init; }

    public IReadOnlyDictionary<DeliveryChannel, ChannelSettings> Channels { get; init; }
}
