namespace MyNUnitSpace;

/// <summary>
/// Class for information about test
/// </summary>
public class TestInfo
{
    /// <summary>
    /// Name of test
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Result of test
    /// </summary>
    public TestState Result { get; }

    /// <summary>
    /// Elapsed time for test
    /// </summary>
    public TimeSpan Time { get; }

    /// <summary>
    /// Message for user about test
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Result of the test states
    /// </summary>
    public enum TestState
    {
        Passed,
        Failed,
        Ignored
    }

    public TestInfo(string name, TestState result, TimeSpan time, string message)
    {
        this.Name = name;
        this.Result = result;
        this.Time = time;
        this.Message = message;
    }

    public TestInfo(string name, TestState result, TimeSpan time)
    {
        this.Name = name;
        this.Result = result;
        this.Time = time;
    }
}
