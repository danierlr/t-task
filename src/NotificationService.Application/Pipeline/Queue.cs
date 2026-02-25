namespace NotificationService.Application.Pipeline;

// Custom queue so that remove operation is O(1)

public class Node<T> {
    public Node<T>? Prev;
    public Node<T>? Next;
}

public class Queue<T> {
    public Node<T>? First = null;
    public Node<T>? Last = null;

    private long _count = 0;

    public Queue<T>(){
        // todo
    }

oublic 
}
