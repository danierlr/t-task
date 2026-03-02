namespace NotificationService.Infrastructure.RetryQueue;

// This is quick and dirty priority queue for in-memory retry queue that has "remove" operation (flaw: keeps the removed stuff until it is dequeued)
public class MinPriorityQueueRemovable<TElement, TPriority>
    where TPriority : IComparable<TPriority>
    where TElement : IEquatable<TElement> {
    private readonly HashSet<TElement> _removed = new();
    private readonly PriorityQueue<TElement, TPriority> _queue = new();

    public void Enqueue(TElement element, TPriority priority) {
        // assuming the existing element wont be enqueued again
        _removed.Remove(element);
        _queue.Enqueue(element, priority);
    }

    public bool TryDequeue(out TElement element) {
        element = default!;

        bool exists = TryPeek(out element);

        if (exists) {
            exists = _queue.TryDequeue(out element, out _);
        }

        return exists;
    }

    public bool TryPeek(out TElement element) {
        element = default!;

        while (true) {
            bool topExists = _queue.TryPeek(out var topElement, out var topPriority);

            if (!topExists || topPriority is null || topElement is null) {
                return false;
            }

            if (_removed.Contains(topElement)) {
                _removed.Remove(topElement);
                _queue.TryDequeue(out topElement, out topPriority);
                continue;
            }

            element = topElement;

            return true;
        }
    }

    public void Remove(TElement element) {
        _removed.Add(element);
    }
}
