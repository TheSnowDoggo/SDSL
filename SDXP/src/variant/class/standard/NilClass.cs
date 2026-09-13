namespace SDSL;

[ClassExport]
public static class NilClass
{
	public static VariantClass Class { get; } = new VariantClass("Nil", VariantType.Nil);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(NilClass), Class);
	}
}