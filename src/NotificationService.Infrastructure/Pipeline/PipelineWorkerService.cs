using Microsoft.Extensions.Hosting;
using NotificationService.Application.Pipeline;

namespace NotificationService.Infrastructure.Pipeline;

internal class PipelineWorkerService : IHostedService {
    private readonly NotificationPipeline _notificationPipeline;
    private Thread? _thread;
    private readonly PipelineCancelToken _pipelineCancelToken;

    public PipelineWorkerService(NotificationPipeline notificationPipeline, PipelineCancelToken pipelineCancelToken) {
        _notificationPipeline = notificationPipeline;
        _pipelineCancelToken = pipelineCancelToken;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _thread = new Thread(() => {
            _notificationPipeline.Run();
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

        _pipelineCancelToken.Cancel();
        _thread?.Join();
        _pipelineCancelToken.Dispose();

        return Task.CompletedTask;
    }
}
