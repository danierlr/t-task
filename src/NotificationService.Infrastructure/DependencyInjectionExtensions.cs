using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.IdGenerator;
using NotificationService.Application.Notifications;
using NotificationService.Domain.Ports;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure;

public static class DependencyInjectionExtensions {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddSingleton<NotificationSettleRegistry>();
        services.AddSingleton<IIdGenerator, SequentialIdGenerator>();
        services.AddSingleton<INotificationSender, NotificationSender>();
        services.AddSingleton<INotificationRepository, FakeNotificationRepository>();

        return services;
    }
}
