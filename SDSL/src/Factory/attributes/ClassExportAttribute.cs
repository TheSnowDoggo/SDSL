namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Class)]
public class ClassExportAttribute : Attribute
{
	public string GenerateMethodName { get; init; }
}