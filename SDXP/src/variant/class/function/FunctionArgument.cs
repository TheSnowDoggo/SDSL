namespace SDSL;

public readonly struct FunctionArgument
{
	public FunctionArgument(
		string name,
		VariantClass variantClass)
	{
		Name = name;
		VariantClass = variantClass;
	}
	
	public string Name { get; }
	public VariantClass VariantClass { get; }

	public override string ToString()
	{
		return VariantClass == null
			? $"{Name}: Any"
			: $"{Name}: {VariantClass}";
	}
}