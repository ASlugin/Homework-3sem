namespace MyThreadPool.Tests;

public class Tests
{
    private int amountOfTasks = 30;

    private MyThreadPool threadPool;
    private IMyTask<int>[] tasks;
    private int[] resultForTask;

    [SetUp]
    public void Setup()
    {
        threadPool = new MyThreadPool(Environment.ProcessorCount);
        tasks = new IMyTask<int>[amountOfTasks];
        resultForTask= new int[amountOfTasks];

        var functions = new Func<int>[amountOfTasks];
        for (int i = 0; i < amountOfTasks; i++)
        {
            var localI = i;
            functions[i] = () =>
            {
                var sum = 0;
                for (int j = 0; j < 100; ++j)
                {
                    sum++;
                }
                return sum + localI * 2;
            };
            resultForTask[i] = 100 + localI * 2;
        }
        for (int i = 0; i < amountOfTasks; i++)
        {
            tasks[i] = threadPool.Submit(functions[i]);
        }
    }
    
    [Test]
    public void ThreadPoolShallSolveTasksCorrectly()
    {
        for (int i = 0; i < amountOfTasks; i++)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(resultForTask[i]));
        }
    }
    
    [Test]
    public void ThreadPoolShallCalculateSubmittedTasksAfterShutdown()
    {
        threadPool.Shutdown();
        for (int i = 0; i < amountOfTasks; i++)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(resultForTask[i]));
        }
    }

    [Test]
    public void NewTasksCannotBeSumbittedAfterShutdown()
    {
        threadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 10));
    }

    [Test]
    public void ThreadPoolShallCalculateContinueCorrectly()
    {
        var continueTasks = new IMyTask<string>[amountOfTasks];
        for (int i = 0; i < amountOfTasks; ++i)
        {
            continueTasks[i] = tasks[i].ContinueWith(result => String.Concat("Result: ", result.ToString()));
        }
        for (int i = 0; i < amountOfTasks; ++i)
        {
            Assert.That(continueTasks[i].Result, Is.EqualTo($"Result: {resultForTask[i]}"));
        }
    }

    [Test]
    public void ThreadPoolShallCalculateContinueTasksAfterShutdown()
    {
        var continueTasks = new IMyTask<string>[amountOfTasks];
        for (int i = 0; i < amountOfTasks; ++i)
        {
            continueTasks[i] = tasks[i].ContinueWith(result => String.Concat("Result: ", result.ToString()));
        }
        threadPool.Shutdown();
        for (int i = 0; i < amountOfTasks; ++i)
        {
            Assert.That(continueTasks[i].Result, Is.EqualTo($"Result: {resultForTask[i]}"));
        }
    }

    [Test]
    public void ContinueTaskCannotBeSubmittedAfterShutdown()
    {
        threadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => tasks[0].ContinueWith<int>(result => result * 1));
    }

    [Test]
    public void ResultOfTaskShallThrowsAggregateExceptionIfAtTaskExceptionWasThrown()
    {
        var task = threadPool.Submit(new Func<int>(() => { throw new DivideByZeroException(); }));
        Assert.Throws<AggregateException>(() => { var a = task.Result; });
    }
}
