namespace NotificationService.Application.Configuration;


public record ChannelConfiguration {
    /// <summary>
    /// How many retries with the external provider should be attempted
    /// </summary>
    public int MaxNumRetries { get; init; }

    /// <summary>
    /// How long the send request should linger arround waiting for the notification to be sent before returning
    /// </summary>
    public int InitialResultTimeoutMs { get; init; }

    /// <summary>
    /// After how many milliseconds transition notification to a failed state
    /// </summary>
    public int NotificationGlobalTimeoutMs { get; init; }
}
