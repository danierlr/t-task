namespace NotificationService.Application.Shared;

public readonly record struct TimeoutResult<T>(bool Completed, T Value);
