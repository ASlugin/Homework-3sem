namespace Lazy;

/// <summary>
/// Realizes lazy evaluation in singlethreaded mode
/// </summary>
public class SingleThreadedLazy<T> : ILazy<T>
{
    private Func<T?>? supplier;

    private T? result;
    private bool wasResultReceived = false;

    public SingleThreadedLazy(Func<T?> supplier)
    {
        if (supplier is null)
        {
            throw new ArgumentNullException("Function can't be null");
        }
        this.supplier = supplier;
    }

    public T? Get()
    {
        if (!wasResultReceived)
        {
            result = supplier!();
            wasResultReceived = true;
            supplier = null;
        }
        return result;
    }
}