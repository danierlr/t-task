namespace NotificationService.Application.Pipeline;

public class PipelineCancelToken: IDisposable {
    private readonly CancellationTokenSource _cts = new();

    public CancellationToken Token => _cts.Token;

    public void Cancel() => _cts.Cancel();

    public void Dispose() => _cts.Dispose();
}
