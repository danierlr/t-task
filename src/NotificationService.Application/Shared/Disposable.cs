namespace NotificationService.Application.Shared;

public class Disposable : IDisposable {
    private Action _dispose;

    public Disposable(Action dispose) {
        _dispose = dispose;
    }

    public void Dispose() {
        _dispose();
    }
}
