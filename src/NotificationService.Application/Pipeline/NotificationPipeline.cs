using NotificationService.Application.Notifications;
using NotificationService.Application.RetryQueue;
using NotificationService.Application.Shared;
using System.Threading.Channels;

namespace NotificationService.Application.Pipeline;

public class NotificationPipeline {
    private readonly CapacityLimiter _globalCapacityLimiter = new CapacityLimiter(10000);

    private readonly Channel<NotificationEntry> _inbound;

    //private readonly LinkedList<string> _inboundBuffer = new();
    private readonly IRetryQueue _retryQueue;

    private readonly AutoResetEvent _autoResetEvent = new(false);

    public NotificationPipeline(IRetryQueue retryQueue) {
        _retryQueue = retryQueue;
        _inbound = Channel.CreateUnbounded<NotificationEntry>();
    }

    public bool TrySubmitNew(NotificationEntry notification) {
        bool reserved = _globalCapacityLimiter.TryReserve(1);

        if (!reserved) {
            return false;
        }

        bool written = _inbound.Writer.TryWrite(notification);

        if (!written) {
            _globalCapacityLimiter.Release(1);
            throw new InvalidOperationException("Could not write to unbound channel");
        }

        Wake();

        return true;
    }

    public void SubmitRetry(NotificationEntry notification) {
        throw new NotImplementedException();
    }

    private void ProcessExpired() {
        throw new NotImplementedException();
    }

    private void ProcessRetry() {
        throw new NotImplementedException();
    }

    private void ProcessInbox() {
        throw new NotImplementedException();
    }

    private TimeSpan FindSleepDuration() {
        throw new NotImplementedException();
        //return Timeout.InfiniteTimeSpan;

    }

    private void Sleep(TimeSpan duration) {
        _autoResetEvent.WaitOne(duration);
    }

    private void Wake() {
        _autoResetEvent.Set();
    }

    public void Run(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            ProcessExpired();
            ProcessRetry();
            ProcessInbox();

            TimeSpan duration = FindSleepDuration();
            Sleep(duration);
        }
    }
}
