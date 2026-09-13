namespace SDSL;

public class UserVariantObject : VariantObject
{
	public UserVariantObject(
		VariantClass variantClass,
		Variant[] fields)
	{
		TypeClass = variantClass;
		Fields = fields;
	}
	
	public override VariantClass TypeClass { get; }
	
	public Variant[] Fields { get; set; }
}