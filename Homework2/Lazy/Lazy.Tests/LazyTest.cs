namespace LazyTest;

using Lazy;

public class Tests
{
    [Test]
    public void GetReturnsSameValueForAnyCall()
    {
        var value = 7;
        var lazy = new SingleThreadedLazy<int>(() => value);
        var lazyMultithreded = new MultiThreadedLazy<int>(() => value);

        for (var i = 0; i < 5; ++i)
        {
            Assert.That(value, Is.EqualTo(lazy.Get()));
            Assert.That(value, Is.EqualTo(lazyMultithreded.Get()));
        }
    }

    [Test]
    public void GetReturnsSameObjectForAnyCall()
    {
        var testString = "Hello World!";
        var lazy = new SingleThreadedLazy<string>(() => testString);
        var lazyMultithreded = new MultiThreadedLazy<string>(() => testString);
        for (var i = 0; i < 5; ++i)
        {
            Assert.That(testString, Is.SameAs(lazy.Get()));
            Assert.That(testString, Is.SameAs(lazyMultithreded.Get()));
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