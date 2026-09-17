namespace SDSL;

[AttributeUsage(AttributeTargets.Method)]
public class ConstructorExportAttribute : FunctionExportAttribute
{
	public ConstructorExportAttribute(params string[] argumentTypes)
		: base(argumentTypes) { }
}