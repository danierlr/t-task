namespace NotificationService.Application.Shared;

// Similar to semaphore slim with 0 timeout (never blocks, no contention), differences: changeable capacity, can lock and release more than 1

internal class CapacityLimiter {
    private long _capacity;
    private long _count = 0;

    public CapacityLimiter(long initialCapacity) {
        _capacity = initialCapacity;
    }

    public long Count => Interlocked.Read(ref _count);
    public long Capacity => Interlocked.Read(ref _capacity);

    public bool TryReserve(long reserveCount) {
        var newCount = Interlocked.Add(ref _count, reserveCount);

        if(newCount > Capacity) {
            Interlocked.Add(ref _count, reserveCount * -1);
            return false;
        }

        return true;
    }

    public void Release(long releaseCount) {
        Interlocked.Add(ref _count, releaseCount * -1);
    }

    public void SetCapacity(long newCapacity) {
        Interlocked.Exchange(ref _capacity, newCapacity);
    }
}
