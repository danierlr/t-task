using Microsoft.Extensions.Hosting;
using NotificationService.Application.Initialization;

namespace NotificationService.Infrastructure.Initialization;

internal class InitializationHostedService: IHostedService {
    private readonly InitializationService _initializationService;

    public InitializationHostedService(InitializationService initializationService) {
        _initializationService = initializationService;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        await _initializationService.Initialize(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}
