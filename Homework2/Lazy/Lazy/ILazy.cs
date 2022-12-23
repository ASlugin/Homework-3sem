namespace Lazy;

/// <summary>
/// Interface for realization class with lazy evaluation
/// </summary>
public interface ILazy<T>
{
    /// <summary>
    /// First time it evaluates and saves result, then return this result without evaluating 
    /// </summary>
    public T? Get();
}