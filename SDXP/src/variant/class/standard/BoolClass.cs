namespace SDSL;

[ClassExport]
public static class BoolClass
{
	public static VariantClass Class { get; } = new VariantClass("Bool", VariantType.Bool);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(BoolClass), Class);
	}
}