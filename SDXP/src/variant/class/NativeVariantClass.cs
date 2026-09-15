namespace SDSL;

public class NativeVariantClass : VariantClass
{
	public NativeVariantClass(
		string name,
		VariantType variantType,
		VariantClass baseClass)
	{
		Name = name;
		VariantType = variantType;
		BaseClass = baseClass;
	}

	public NativeVariantClass(
		string name,
		VariantType variantType = VariantType.Object)
	{
		Name = name;
		VariantType = variantType;
		BaseClass = ObjectClass.Class;
	}
	
	public override Function Constructor => NativeConstructor;
	
	public NativeFunction NativeConstructor { get; set; }
}