namespace NotificationService.Domain.ValueObjects;

/// <summary>
/// Configuration for a notification provider within a channel.
/// </summary>
/// <param name="Enabled">Whether this provider is enabled for the relevant channel.</param>
/// <param name="Priority">Priority for the relevant channel. Higher number = higher priority.</param>
public record ProviderConfiguration(
    bool Enabled,
    int Priority
);
