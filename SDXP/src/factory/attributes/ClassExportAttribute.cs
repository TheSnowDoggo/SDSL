namespace SDSL;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ClassExportAttribute : Attribute
{
	public string GenerateMethodName { get; init; }
}
