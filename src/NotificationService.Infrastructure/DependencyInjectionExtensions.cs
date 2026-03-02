using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.IdGenerator;
using NotificationService.Application.Notifications;
using NotificationService.Application.Settings;
using NotificationService.Domain.Ports;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Settings;

namespace NotificationService.Infrastructure;

public static class DependencyInjectionExtensions {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        var config = configuration.Get<PipelineConfiguration>();

        var initialSettings = SettingsMapper.ToPipelineSettings(config);

        services.AddSingleton<PipelineSettings>(initialSettings);
        services.AddSingleton<SettingsService>();

        services.AddSingleton<NotificationSettleRegistry>();
        services.AddSingleton<IIdGenerator, SequentialIdGenerator>();
        services.AddSingleton<INotificationSender, NotificationSender>();
        services.AddSingleton<INotificationRepository, FakeNotificationRepository>();

        return services;
    }
}
