namespace SDSL.Native;

[ClassExport]
public static class NilClass
{
	public static NativeClass Class { get; } = new NativeClass("Nil", VariantType.Nil);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(NilClass), Class);
	}
}