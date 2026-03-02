namespace NotificationService.Application.Shared;

public static class TimeUtils {
    public static DateTime Min(DateTime first, DateTime second) {
        return first < second ? first : second;
    }

    public static TimeSpan Max(TimeSpan first, TimeSpan second) {
        return first < second ? second : first;
    }
}
