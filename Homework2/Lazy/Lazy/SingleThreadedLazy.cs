namespace Lazy;

/// <summary>
/// Realizes lazy evaluation in singlethreaded mode
/// </summary>
public class SingleThreadedLazy<T> : ILazy<T>
{
    private Func<T?> supplier;

    private T? result;
    private bool wasResultReceived = false;

    public SingleThreadedLazy(Func<T?> supplier)
        => this.supplier = supplier;

    public T? Get()
    {
        if (!wasResultReceived)
        {
            result = supplier();
            wasResultReceived = true;
        }
        return result;
    }
}