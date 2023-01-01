namespace TestProject;

using Attributes;

public class FailedAfterClass
{
    [Test(Expected = typeof(DivideByZeroException))]
    public void TestWhenAfterClassFailed()
    {
        var a = 1;
        var b = 2;
        var result = a / (b - 2);
    }

    [AfterClass]
    public static void AfterClassFail()
    {
        throw new Exception();
    }
}
