namespace QueueTests;
using Queue;
using System.Collections.Generic;

public class Tests
{
    [Test]
    public void QueueWithPriorityShallWorkCorrectlyWhenOneThread()
    {
        var queue = new QueueWithPriority<int>();

        for (int i = 0; i < 11; ++i)
        {
            queue.Enqueue(i, i);
        }

        for (int i = 10; i >= 0; --i)
        {
            Assert.That(queue.Dequeue(), Is.EqualTo(i));
        }
    }

    [Test]
    public void QueueWithPriorityShallWorkCorrectlyWhenMultiThread()
    {
        var queue = new QueueWithPriority<int>();

        var threads = new Thread[Environment.ProcessorCount];
        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 10; ++j)
                {
                    queue.Enqueue(j, j);
                }
            });
        }
        
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.That(queue.Size(), Is.EqualTo(Environment.ProcessorCount * 10));
    }
}