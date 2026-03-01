namespace NotificationService.Application.Shared;

public readonly record struct TimeoutResult<T>(bool Completed, T Value);

public static class AsyncExtensions {
    public static async Task<TimeoutResult<T>> WithTimeout<T>(this Task<T> task, TimeSpan timeout) {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cancellationTokenSource.Token));

        if (completedTask == task) {
            cancellationTokenSource.Cancel();
            return new TimeoutResult<T>(true, await task);
        }

        return new TimeoutResult<T>(false, default);
    }
}
