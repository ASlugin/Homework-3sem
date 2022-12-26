namespace MyNUnitSpace.Tests;

using TestProject;

public class Tests
{
    [Test]
    public void MyNUnitShallWorkCorrectly()
    {
        var nUnit = new MyNUnit();
        nUnit.RunTests("../../../../TestProject/bin/");

        var answer = new List<(string name, TestInfo.TestState result)> 
        {
            ("Test1", TestInfo.TestState.Passed),
            ("Test2", TestInfo.TestState.Passed),
            ("IgnorTest", TestInfo.TestState.Ignored),
            ("TestWhenBeforeClassFailed", TestInfo.TestState.Ignored),
            ("ExceptionTest1", TestInfo.TestState.Passed),
            ("ExceptionTestThatNotThrowException", TestInfo.TestState.Failed),
            ("ExceptionTestThatThrowOtherException", TestInfo.TestState.Failed),
            ("TestWhenAfterClassFailed", TestInfo.TestState.Passed),
            ("TestWhenAfterClassFailed", TestInfo.TestState.Failed),
            ("TestWhenAfterFailed", TestInfo.TestState.Failed)
        };

        foreach (var resultOfTest in nUnit.TestResults)
        {
            Assert.IsTrue(answer.Contains(new(resultOfTest.Name, resultOfTest.Result)));
        }
    }
}