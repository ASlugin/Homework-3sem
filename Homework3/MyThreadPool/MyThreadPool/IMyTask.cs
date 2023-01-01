namespace MyThreadPool;

/// <summary>
/// Interface for task that will be returned by thread pool
/// </summary>
public interface IMyTask<TResult>
{
    /// <summary>
    /// True if task is calculated, false - not yet
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Result of calculating task
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Adds new task, which depends on source task
    /// </summary>
    /// <returns>Returns new task</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask);
}
