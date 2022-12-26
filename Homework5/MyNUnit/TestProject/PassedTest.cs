namespace TestProject;

using Attributes;

public class ClassForPassedTest
{
    private static int b = 0;

    [BeforeClass]
    public static void BeforeClassTest1()
    {
        var a = b;
        for (int i = 0; i < 100; i++)
        {
            a += i;
        }
        b = a;
    }

    [BeforeClass]
    public static void BeforeClassTest2()
    {
        if (b == 0)
        {
            throw new Exception();
        }
    }

    [Before]
    public void BeforeTests()
    {
        b = 10000;
    }

    [Test]
    public void Test1()
    {
        if (b != 10000)
        {
            throw new Exception();
        }
    }

    [Test(Expected = typeof(DivideByZeroException))]
    public void Test2()
    {
        var a = 1 / (b - 10000);
    }

    [Test(Ignore = "Ignore this test")]
    public void IgnorTest()
    {
        var a = 1 / (b - 10000);
    }

    [After]
    public void AfterEachTest()
    {
        b = -100;
    }

    [AfterClass]
    public static void AfterClassTest()
    {
        if (b > 0)
        {
            throw new ArgumentException();
        }
    }
}