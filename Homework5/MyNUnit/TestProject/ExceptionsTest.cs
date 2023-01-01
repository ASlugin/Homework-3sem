namespace TestProject;

using Attributes;

public class ClassForTestExceptions
{

    [Test(Expected = typeof(ArgumentOutOfRangeException))]
    public void ExceptionTest1()
        => throw new ArgumentOutOfRangeException();

    [Test(Expected = typeof(Exception))]
    public void ExceptionTestThatNotThrowException()
    {
        var a = "Hello world";
        a += "aaaa";
    }

    [Test(Expected = typeof(DivideByZeroException))]
    public void ExceptionTestThatThrowOtherException()
    {
        throw new InsufficientMemoryException();
    }
}
