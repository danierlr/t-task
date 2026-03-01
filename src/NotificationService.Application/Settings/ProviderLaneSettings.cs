using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Settings;

public record ProviderLaneSettings {
    public ProviderConfiguration Provider { get; init; }

    /// <summary>
    /// How many max requests can be active at the time for the particular channel and particular provider
    /// </summary>
    public long NumConcurrencySlots { get; init; }

    /// <summary>
    /// How many notifications can be waiting in queue for the particular provider and particular channel
    /// </summary>
    public long BufferCapacity { get; init; }

    /// <summary>
    /// Duration after which request sent to the external provider times out
    /// </summary>
    public TimeSpan SendTimeout { get; init; }
}
