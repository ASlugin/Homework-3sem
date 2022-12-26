namespace Attributes;

/// <summary>
/// Attribute for method that should run once before testing
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute
{

}