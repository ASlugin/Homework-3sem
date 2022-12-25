namespace MyThreadPool;

using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// Class for thread pool
/// </summary>
public class MyThreadPool : IDisposable
{
    private ConcurrentQueue<Action> tasks;
    private Thread[] threads;
    private CancellationTokenSource cancellationTokenSource;

    private AutoResetEvent threadPoolEvent;

    private int tasksCount = 0;

    public MyThreadPool(int amountOfThreads)
    {
        this.tasks = new();
        this.cancellationTokenSource = new();
        this.threadPoolEvent = new AutoResetEvent(false);

        threads = new Thread[amountOfThreads];
        for (int i = 0; i < amountOfThreads; i++)
        {
            threads[i] = CreateThread();
            threads[i].Start();
        }
    }

    private Thread CreateThread()
    {
        var thread = new Thread(() =>
        {
            while (true)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested && tasksCount == 0)
                {
                    return;
                }

                threadPoolEvent.WaitOne();

                if (!tasks.IsEmpty || cancellationTokenSource.Token.IsCancellationRequested)
                {
                    threadPoolEvent.Set();
                }

                if (tasks.TryDequeue(out Action? task))
                {
                    task();
                    Interlocked.Decrement(ref tasksCount);
                }
            }
        });
        return thread;
    }

    /// <summary>
    /// Adds task to queue for calculating in thread pool
    /// </summary>
    /// <returns></returns>
    public IMyTask<T> Submit<T>(Func<T> task)
    {
        if (cancellationTokenSource.Token.IsCancellationRequested)
        {
            throw new InvalidOperationException();
        }

        var newTask = new MyTask<T>(task, this);
        tasks.Enqueue(newTask.Run);
        Interlocked.Increment(ref tasksCount);
        threadPoolEvent.Set();

        return newTask;
    }

    /// <summary>
    /// Finishes work of thread pool
    /// </summary>
    public void Shutdown()
    {
        cancellationTokenSource.Cancel();

        threadPoolEvent.Set();
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i].Join();
        }
    }

    public void Dispose()
    {
        threadPoolEvent.Dispose();
    }

    /// <summary>
    /// Class for task, implementation of interface IMyTask
    /// </summary>
    private class MyTask<T> : IMyTask<T>
    {
        private T? result;
        private bool isCompleted;

        private ManualResetEvent resultIsReceived;

        private Exception? exception;

        private MyThreadPool threadPool;

        private Func<T>? task;

        private ConcurrentQueue<Action> continueTasks;

        
        public MyTask(Func<T> task, MyThreadPool threadPool)
        {
            this.task = task;
            this.threadPool = threadPool;
            this.continueTasks = new();
            this.resultIsReceived = new ManualResetEvent(false);
        }

        public bool IsCompleted
            => isCompleted;

        public T Result
        {
            get
            {
                if (!isCompleted)
                {
                    resultIsReceived.WaitOne();
                }

                if (exception != null)
                {
                    throw exception;
                }
                return result!;
            }
        }

        /// <summary>
        /// Starts of calculating of task
        /// </summary>
        public void Run()
        {
            try
            {
                result = task!();
            }
            catch (Exception exc)
            {
                exception = new AggregateException(exc);
            }
            

            task = null;
            isCompleted = true;
            resultIsReceived.Set();

            while (!continueTasks.IsEmpty)
            {
                if (continueTasks.TryDequeue(out Action? continueTask))
                {
                    threadPool.tasks.Enqueue(continueTask);
                    threadPool.threadPoolEvent.Set();
                }
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T, TNewResult> continueTask)
        {
            if (threadPool.cancellationTokenSource.Token.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }

            Func<TNewResult> continueTaskForThreadPool = () =>
            {
                if (exception != null)
                {
                    throw exception;
                }
                return continueTask(result!);
            };

            if (isCompleted)
            {
                return threadPool.Submit(continueTaskForThreadPool);
            }

            var newContinueTask = new MyTask<TNewResult>(continueTaskForThreadPool, threadPool);
            continueTasks.Enqueue(newContinueTask.Run);
            Interlocked.Increment(ref threadPool.tasksCount);
            return newContinueTask;
        }
    }
}