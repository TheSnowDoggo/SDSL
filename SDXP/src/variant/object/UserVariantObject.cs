namespace SDSL;

public class UserVariantObject : VariantObject
{
	public UserVariantObject(
		VariantClass variantClass,
		Variant[] fields,
		VariantObject compoundBase)
	{
		ParentClass = variantClass;
		Fields = fields;
		CompoundBase = compoundBase;
	}
	
	public override VariantClass ParentClass { get; }
	
	public Variant[] Fields { get; set; }
	
	public VariantObject CompoundBase { get; }
}