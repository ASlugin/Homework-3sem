namespace Lazy;

using System.Threading;

/// <summary>
/// Realizes lazy evaluation in multithreaded mode
/// </summary>
public class MultiThreadedLazy<T> : ILazy<T>
{
    private Func<T?>? supplier;

    private T? result;
    private volatile bool wasResultReceived = false;

    private Object lockObject = new();

    public MultiThreadedLazy(Func<T?> supplier)
    {
        if (supplier is null)
        {
            throw new ArgumentNullException("Function can't be null");
        }
        this.supplier = supplier;
    }

    public T? Get()
    {
        if (wasResultReceived)
        {
            return result;
        }
        lock (lockObject)
        {
            if (!wasResultReceived)
            {
                result = supplier!();
                wasResultReceived = true;
                supplier = null;
            }
        }
        return result;
    }
}