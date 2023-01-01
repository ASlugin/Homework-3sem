namespace TestProject;

using Attributes;

public class FailedBeforeClass
{
    [BeforeClass]
    public static void BeforeMethodFailed()
    {
        throw new ArgumentOutOfRangeException();
    }

    [Test]
    public void TestWhenBeforeClassFailed()
    {
        var a = 1;
        var b = 2;
        var result = a * b * (a + b);
    }
}
