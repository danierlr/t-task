using NotificationService.Application.Settings;
using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Infrastructure.Settings;

public static class SettingsMapper {
    public static PipelineSettings ToPipelineSettings(PipelineConfiguration configuration) {
        Dictionary<(DeliveryChannel Channel, string ProviderName), ProviderLaneSettings> lanes = new();
        Dictionary<DeliveryChannel, ChannelSettings> channels = new();

        foreach (var channelConfiguration in configuration.Channels) {
            ChannelSettings channelSettings = new() {
                MaxNumRetries = channelConfiguration.Value.MaxNumRetries,
                ResultLingerDuration = TimeSpan.FromMilliseconds(channelConfiguration.Value.ResultLingerDurationMs),
                TimeToLive = TimeSpan.FromMilliseconds(channelConfiguration.Value.TimeToLiveMs),
                InitialRetryDelay = TimeSpan.FromMilliseconds(channelConfiguration.Value.InitialRetryDelay),
                BackoffMultiplier = channelConfiguration.Value.BackoffMultiplier,
                MaxRetryDelay = TimeSpan.FromMilliseconds(channelConfiguration.Value.MaxRetryDelayMs),
            };

            channels.Add(channelConfiguration.Key, channelSettings);

            foreach(var providerConfiguration in channelConfiguration.Value.Providers) {
                ProviderConfiguration provider = new(
                    Enabled: providerConfiguration.Value.Enabled,
                    Priority: providerConfiguration.Value.Priority
                );

                ProviderLaneSettings laneSettings = new() {
                    Provider = provider,
                    NumConcurrencySlots = providerConfiguration.Value.NumConcurrencySlots,
                    BufferCapacity = providerConfiguration.Value.BufferCapacity,
                    SendTimeout = TimeSpan.FromMilliseconds(providerConfiguration.Value.SendTimeoutMs)
                };

                lanes.Add((channelConfiguration.Key, providerConfiguration.Key), laneSettings);
            }
        }

        PipelineSettings settings = new() {
            TotalNotificationCapacity = configuration.TotalNotificationCapacity,
            ConfigurationUpdateTimeout = TimeSpan.FromMilliseconds(configuration.ConfigurationUpdateTimeoutMs),
            Lanes = lanes,
            Channels = channels,
        };

        return settings;
    }
}
