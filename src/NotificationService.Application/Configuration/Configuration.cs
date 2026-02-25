using NotificationService.Domain.Aggregates.Notifications;

namespace NotificationService.Application.Configuration;

public record Configuration {
    /// <summary>
    /// How many notifications can be queued in total
    /// </summary>
    public long GlobalNotificationCapacity {get; init; }

    // TODO fix the name later
    /// <summary>
    /// How long to wait for the pipeline worker to pause
    /// </summary>
    public int ConfigurationUpdateTimeoutMs { get; init; }

    public IReadOnlyDictionary<(DeliveryChannel Channel, string ProviderName), ProviderLaneConfiguration> Lanes { get; init; }

    public IReadOnlyDictionary<DeliveryChannel, ChannelConfiguration> Channels { get; init; }
}
