namespace SDSL;

public class FunctionArgument
{
	public FunctionArgument(
		string name,
		string prototypeClass)
	{
		Name = name;
		PrototypeClass = prototypeClass;
	}
	
	public string Name { get; }
	
	public VariantClass VariantClass { get; set; }
	
	public string PrototypeClass { get; set; }

	public override string ToString()
	{
		return VariantClass == null
			? $"{Name}: Any"
			: $"{Name}: {VariantClass}";
	}
}