namespace NotificationService.Infrastructure.Settings;

public class ProviderLaneConfiguration {
    public bool Enabled { get; set; }

    public long Priority { get; set; }

    public long NumConcurrencySlots { get; set; }

    public long BufferCapacity { get; set; }

    public long SendTimeoutMs { get; set; }
}
