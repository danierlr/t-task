namespace NotificationService.Application.Settings;

public record ChannelSettings {
    /// <summary>
    /// How many retries with the external provider should be attempted at most before the notification should be considered failed
    /// </summary>
    public long MaxNumRetries { get; init; }

    /// <summary>
    /// How long the send request should linger arround waiting for the notification to be sent before returning
    /// </summary>
    public TimeSpan ResultLingerDuration { get; init; }

    /// <summary>
    /// Duration after which notification should be considered expired
    /// </summary>
    public TimeSpan TimeToLive { get; init; }

    /// <summary>
    /// Delay duration for the first delay attempt
    /// </summary>
    public TimeSpan InitialRetryDelay { get; init; }

    /// <summary>
    /// How many times longer the next delay for retry attempt should be
    /// </summary>
    public double BackoffMultiplier { get; init; }

    /// <summary>
    /// Max delay duration for a retry
    /// </summary>
    public TimeSpan MaxRetryDelay { get; init; }
}
