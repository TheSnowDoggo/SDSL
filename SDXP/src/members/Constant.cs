namespace SDSL;

public class Constant
{
	public Constant(
		string name,
		VariantClass variantClass,
		Variant value)
	{
		Name = name;
		VariantClass = variantClass;
		Value = value;
	}
	
	public string Name { get; }
	public VariantClass VariantClass { get; }
	public Variant Value { get; }

	public string FullName => $"{VariantClass}.{Name}";

	public override string ToString()
	{
		return $"const {VariantClass}.{Name} = {Value.ToSafeString()}";
	}
}