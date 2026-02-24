using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Configuration;

public class Configuration {
    public readonly long GlobalNotificationCapacity;
    public readonly double ConfigurationUpdateTimeoutMs;

    Dictionary<(DeliveryChannel Channel, string ProviderName), ProviderConfiguration> Provicers;

    D
}
