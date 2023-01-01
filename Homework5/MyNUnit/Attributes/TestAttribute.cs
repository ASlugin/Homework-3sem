namespace Attributes;

/// <summary>
/// Attribute for method that should run to test
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    public Type? Expected { get; set; }

    public string? Ignore { get; set; }
}