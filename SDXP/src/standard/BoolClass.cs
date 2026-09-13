namespace SDSL;

[ClassExport]
public static class BoolClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("Bool", VariantType.Bool);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(BoolClass), Class);
	}
}