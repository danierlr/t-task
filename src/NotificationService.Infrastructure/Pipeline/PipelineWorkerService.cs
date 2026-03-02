using Microsoft.Extensions.Hosting;
using NotificationService.Application.Pipeline;

namespace NotificationService.Infrastructure.Pipeline;

internal class PipelineWorkerService : IHostedService {
    private readonly NotificationPipeline _notificationPipeline;
    private Thread? _thread;
    private CancellationTokenSource? _cts;

    public PipelineWorkerService(NotificationPipeline notificationPipeline) {
        _notificationPipeline = notificationPipeline;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        // need custom token source, because cancellationToken here serves to abort start only, it is not used for stop
        _cts = CancellationTokenSource.CreateLinkedTokenSource();

        _thread = new Thread(() => {
            _notificationPipeline.Run(_cts.Token);
        }) {
            IsBackground = true,
            Name = "NotificationPipelineWorker",
            Priority = ThreadPriority.AboveNormal,
        };

        _thread.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        // TODO more graceful shutdown

        _cts?.Cancel();
        _thread?.Join();
        _cts?.Dispose();

        return Task.CompletedTask;
    }
}
