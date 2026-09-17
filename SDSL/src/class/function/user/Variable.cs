namespace SDSL;

public struct Variable
{
	public Variable(VariantClass variableClass, Variant defaultValue)
	{
		VariableClass = variableClass;
		Value = defaultValue;
	}
	
	public VariantClass VariableClass { get; }
	public Variant Value;
}