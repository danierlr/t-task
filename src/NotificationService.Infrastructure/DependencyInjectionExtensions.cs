using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.IdGenerator;
using NotificationService.Application.Notifications;
using NotificationService.Application.Pipeline;
using NotificationService.Application.RetryQueue;
using NotificationService.Application.Settings;
using NotificationService.Application.Shared;
using NotificationService.Domain.Aggregates.Notifications;
using NotificationService.Domain.Ports;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Pipeline;
using NotificationService.Infrastructure.Providers.Fake;
using NotificationService.Infrastructure.RetryQueue;
using NotificationService.Infrastructure.Settings;

namespace NotificationService.Infrastructure;

public static class DependencyInjectionExtensions {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        var config = configuration.Get<PipelineConfiguration>();

        var initialSettings = SettingsMapper.ToPipelineSettings(config!);

        services.AddSingleton(initialSettings);

        var allProviders = new List<INotificationProvider> {
            new FakeProvider<SmsRecipient>("twilio", DeliveryChannel.Sms, 0.8, 500, 150),
            new FakeProvider<EmailRecipient>("twilio", DeliveryChannel.Email, 0.9, 200, 50),
            new FakeProvider<PushRecipient>("twilio", DeliveryChannel.Push, 0.98, 100, 10),

            new FakeProvider<SmsRecipient>("vonage", DeliveryChannel.Sms, 0.9, 450, 120),
            new FakeProvider<EmailRecipient>("vonage", DeliveryChannel.Email, 0.95, 250, 100),
            new FakeProvider<PushRecipient>("vonage", DeliveryChannel.Push, 0.95, 125, 30),

            new FakeProvider<SmsRecipient>("always_failing", DeliveryChannel.Sms, 0, 100, 50),
            new FakeProvider<EmailRecipient>("always_failing", DeliveryChannel.Email, 0, 100, 50),
            new FakeProvider<PushRecipient>("always_failing", DeliveryChannel.Push, 0, 100, 50),

            new FakeProvider<SmsRecipient>("always_succeeding", DeliveryChannel.Sms, 1, 100, 50),
            new FakeProvider<EmailRecipient>("always_succeeding", DeliveryChannel.Email, 1, 100, 50),
            new FakeProvider<PushRecipient>("always_succeeding", DeliveryChannel.Push, 1, 100, 50),
        };

        services.AddSingleton(allProviders);

        Dictionary<DeliveryChannel, IRetryQueue> retryQueues = Enum.GetValues<DeliveryChannel>()
            .ToDictionary(
                channel => channel,
                channel => (IRetryQueue)new InMemorySimpleRetryQueue()
            );

        services.AddSingleton(retryQueues);

        services.AddSingleton<ProviderLaneStore>();
        services.AddSingleton<DomainEventDispatcher>();
        services.AddSingleton<NotificationSettleRegistry>();
        services.AddSingleton<INotificationRepository, FakeNotificationRepository>();
        services.AddSingleton<IIdGenerator, SequentialIdGenerator>();
        services.AddSingleton<INotificationSender, NotificationSender>();
        
        services.AddSingleton<SettingsService>();

        services.AddHostedService<PipelineWorkerService>();

        return services;
    }
}
