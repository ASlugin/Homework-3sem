namespace Attributes;

/// <summary>
/// Attribute for method that should run once after testing
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute
{

}
