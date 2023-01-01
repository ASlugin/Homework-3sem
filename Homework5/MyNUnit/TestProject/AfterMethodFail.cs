namespace TestProject;

using Attributes;

public class AfterMethodFail
{
    [Test(Expected = typeof(DivideByZeroException))]
    public void TestWhenAfterFailed()
    {
        var a = 1;
        var b = 2;
        var result = a / (b - 2);
    }

    [After]
    public static void AfterFail()
    {
        throw new Exception();
    }
}
