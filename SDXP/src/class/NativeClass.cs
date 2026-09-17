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
	
	public static NativeClass CreatePrimitive(string name, VariantType variantType)
	{
		return new NativeClass(name, variantType, Native.ObjectClass.Class);
	}
	
	public static NativeClass InheritObject(string name)
	{
		return new NativeClass(name, VariantType.Object, Native.ObjectClass.Class);
	}
	
	public static NativeClass InheritCustom(string name, NativeClass baseClass)
	{
		return new NativeClass(name, VariantType.Object, baseClass);
	}
	
	public override Function Constructor => NativeConstructor;
	public NativeFunction NativeConstructor { get; set; }

	public override IReadOnlyList<Function> LocalFunctions => LocalNativeFunctions;
	public List<NativeFunction> LocalNativeFunctions { get; set; } = [];

	public override IReadOnlyList<Property> LocalProperties => LocalNativeProperties;
	public List<NativeProperty> LocalNativeProperties { get; set; } = [];

	public override IReadOnlyList<Constant> LocalConstants => LocalNativeConstants;
	public List<Constant> LocalNativeConstants { get; set; } = [];
	
	public void CreateConstant(string name, Variant value)
	{
		LocalNativeConstants.Add(new Constant(name, this, value));
	}
}