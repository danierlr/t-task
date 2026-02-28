using NotificationService.Domain.Ports;

namespace NotificationService.Domain.Aggregates.Notifications;

public readonly record struct NotificationId(string Value) {
    private const string _prefix = "notif_";

    public static NotificationId New(IIdGenerator generator) {
        string strId = generator.Generate();

        return new NotificationId($"{_prefix}{strId}");
    }

    public override string ToString() => Value;
}
