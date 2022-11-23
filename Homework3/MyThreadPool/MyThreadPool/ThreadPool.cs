namespace MyThreadPool;

public class MyThreadPool
{
    private Thread[] threads;

    public MyThreadPool(int amountOfThreads)
    {
        if (amountOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("Amount of threads must be positive");
        }
        threads = new Thread[amountOfThreads];
    }

}