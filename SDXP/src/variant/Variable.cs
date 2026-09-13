namespace SDSL;

public struct Variable
{
	public Variable(VariantClass variantClass, Variant defaultValue)
	{
		VariantClass = variantClass;
		Value = defaultValue;
	}
	
	public VariantClass VariantClass { get; }
	public Variant Value;
}