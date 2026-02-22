namespace NotificationService.Api.Shared;

public static class EnumParser {
    private const int MaxDisplayedValues = 40;

    public static T ParseFromSnakeCase<T>(string snakeCaseValue) where T : struct, Enum {
        var squashed = snakeCaseValue.Replace("_", "");

        if (Enum.TryParse<T>(squashed, ignoreCase: true, out T result)) {
            return result;
        }

        var names = Enum.GetNames<T>();
        var valuesDisplayed = string.Join(", ", names.Take(MaxDisplayedValues));
        string suffix = "";

        if (names.Length > MaxDisplayedValues) {
            suffix = $"... ({names.Length} total)";
        }

        throw new ArgumentException($"Unknown {typeof(T).Name}: '{snakeCaseValue}'. Valid values: {valuesDisplayed}{suffix}");
    }
}
