namespace NotificationService.Application.Shared;

public static class Base36Converter {
    private const string Base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string ToBase36(long value) {
        var result = new char[13]; // max length for long in base36 is 13
        int index = result.Length - 1;

        Array.Fill(result, '0');

        while (value > 0) {
            result[index] = Base36Chars[(int)(value % 36)];
            value /= 36;
            index -= 1;
        }

        return new string(result, 0, 13);
    }
}
