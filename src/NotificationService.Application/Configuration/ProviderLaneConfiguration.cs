using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Configuration;

public record ProviderLaneConfiguration {
    public ProviderConfiguration Provider { get; init; }

    /// <summary>
    /// How many max requests can be active at the time for the particular channel and particular provider
    /// </summary>
    public int NumConcurrencySlots { get; init; }

    /// <summary>
    /// How many notifications can be waiting in queue for the particular provider and particular channel
    /// </summary>
    public int BufferCapacity { get; init; }

    /// <summary>
    /// How many milliseconds after send request to the external provider times out
    /// </summary>
    public int SendTimeoutMs { get; init; }
}
