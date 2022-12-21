namespace Queue;

/// <summary>
/// Class for queue with priorities
/// </summary>
public class QueueWithPriority<T>
{
    private List<(T value, int priority)> list;

    /// <summary>
    /// Amount of elements in queue
    /// </summary>
    public int Size { private set; get; }

    public QueueWithPriority()
    {
        this.list = new();
        this.Size = 0;
    }

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
            Size++;
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
            while (Size == 0)
            {
                Monitor.Wait(list);
            }

            result = list[0].value;
            list.RemoveAt(0);
            Size--;
        }
        return result;
    }
}
