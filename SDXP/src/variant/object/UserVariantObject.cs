namespace SDSL;

public class UserVariantObject : VariantObject
{
	public UserVariantObject(
		VariantClass variantClass,
		Variant[] fields)
	{
		Class = variantClass;
		Fields = fields;
	}
	
	public override VariantClass Class { get; }
	
	public Variant[] Fields { get; set; }
}