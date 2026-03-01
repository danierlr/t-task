namespace NotificationService.Infrastructure.Settings;

public class ChannelConfiguration {
    public long MaxNumRetries { get; set; }

    public long ResultLingerDurationMs { get; set; }

    public long TimeToLiveMs { get; set; }

    public Dictionary <string, ProviderLaneConfiguration> Providers { get; set; } = new();
}
