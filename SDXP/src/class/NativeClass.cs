using SDSL.Native;

namespace SDSL;

public class NativeClass : VariantClass
{
	public NativeClass(
		string name,
		VariantType variantType,
		VariantClass baseClass)
	{
		Name = name;
		VariantType = variantType;
		BaseClass = baseClass;
	}

	public NativeClass(
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