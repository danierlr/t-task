namespace NotificationService.Application.Shared;

public interface ILockable {
    SyncRoot SyncRoot { get; }
}

public class SyncRoot { }

public readonly struct LockToken : IDisposable {
    private readonly SyncRoot _syncRoot;

    public LockToken(SyncRoot syncRoot) {
        _syncRoot = syncRoot;
    }

    public void Dispose() {
        Monitor.Exit(_syncRoot);
    }
}

public static class LockExtensions {
    // extension method instead of default interface implementation, because instance would need casting to interface to call the default implementation
    public static LockToken Lock(this ILockable lockable) {
        Monitor.Enter(lockable.SyncRoot);
        return new LockToken(lockable.SyncRoot);
    }
}