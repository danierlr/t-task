namespace NotificationService.Infrastructure.Settings;

public class ChannelConfiguration {
    public long MaxNumRetries { get; set; }

    public long ResultLingerDurationMs { get; set; }

    public long TimeToLiveMs { get; set; }

    public long InitialRetryDelayMs { get; set; }

    public double BackoffMultiplier { get; set; }

    public long MaxRetryDelayMs { get; set; }

    public Dictionary <string, ProviderLaneConfiguration> Providers { get; set; } = new();
}
