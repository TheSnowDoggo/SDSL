using System.Collections.Frozen;

namespace SDSL;

public class NativeVariantClass : VariantClass
{
	public NativeVariantClass(
		string name,
		VariantType variantType,
		VariantClass baseClass = null)
	{
		Name = name;
		VariantType = variantType;
		BaseClass = baseClass ?? ObjectClass.Class;
	}
}