namespace LazyTest;

using Lazy;

public class Tests
{
    private static IEnumerable<TestCaseData> Lazies()
    {
        int value = 7;
        int valueThreaded = 7;
        yield return new TestCaseData(new SingleThreadedLazy<int>(() => value++));
        yield return new TestCaseData(new MultiThreadedLazy<int>(() => Interlocked.Increment(ref valueThreaded)));
    }

    [TestCaseSource(nameof(Lazies))]
    public void GetShallNotChangeValue<T>(ILazy<T> Lazy)
    {
        var result = Lazy.Get();
        for (var i = 0; i < 10; ++i)
        {
            Assert.That(result, Is.EqualTo(Lazy.Get()));
        }
    }

    [Test]
    public void GetCanReturnNull()
    {
        var lazy = new SingleThreadedLazy<object>(() => null);
        var lazyMultithreded = new MultiThreadedLazy<object>(() => null);
        for (var i = 0; i < 10; ++i)
        {
            Assert.IsNull(lazy.Get());
            Assert.IsNull(lazyMultithreded.Get());
        }
    }

    [Test]
    public void LazyWithThreadsGetTest()
    {
        var value = 10;
        var result = value - 1;
        var lazy = new MultiThreadedLazy<int>(() => Interlocked.Decrement(ref value));

        var threads = new Thread[Environment.ProcessorCount];

        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[localI] = new Thread(() =>
            {
                lazy.Get();
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

        Assert.That(result, Is.EqualTo(lazy.Get()));
    }
}