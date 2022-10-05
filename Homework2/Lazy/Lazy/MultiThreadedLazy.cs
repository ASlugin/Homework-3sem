namespace Lazy;

/// <summary>
/// Realizes lazy evaluation in multithreaded mode
/// </summary>
public class MultiThreadedLazy<T> : ILazy<T>
{
    private Func<T?> supplier;

    private T? result;
    private volatile bool wasResultReceived = false;

    private Object lockObject = new Object();

    public MultiThreadedLazy(Func<T?> supplier)
    {
        this.supplier = supplier;
    }

    public T? Get()
    {
        lock (lockObject)
        {
            if (!wasResultReceived)
            {
                result = supplier();
                wasResultReceived = true;
            }
        }
        return result;
    }
}