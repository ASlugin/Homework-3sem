namespace Queue;

/// <summary>
/// Class for queue with priorities
/// </summary>
public class QueueWithPriority<T>
{
    private List<(T value, int priority)> list;
    private volatile int size = 0;

    public QueueWithPriority()
    {
        this.list = new();
    }

    /// <summary>
    /// Returns amount of elements in queue
    /// </summary>
    public int Size()
        => size;

    /// <summary>
    /// Adds element to queue according to priority
    /// </summary>
    public void Enqueue(T value, int priority)
    {
        lock (list)
        {
            var index = 0;
            var length = list.Count;
            while (index < length && priority <= list[index].priority)
            {
                index++;
            }
            list.Insert(index, (value, priority));
            size++;
            Monitor.PulseAll(list);
        }
    }

    /// <summary>
    /// Returns and removes element from queue with the highest priority
    /// </summary>
    public T Dequeue()
    {
        T result;
        lock (list)
        {
            while (size == 0)
            {
                Monitor.Wait(list);
            }

            result = list[0].value;
            list.RemoveAt(0);
            size--;
        }
        return result;
    }
}