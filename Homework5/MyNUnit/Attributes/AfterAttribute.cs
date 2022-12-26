namespace Attributes;

/// <summary>
/// Attribute for method that should run after each test
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute
{

}
