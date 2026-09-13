namespace SDSL;

[ClassExport]
public static class NilClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("Nil", VariantType.Nil);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(NilClass), Class);
	}
}