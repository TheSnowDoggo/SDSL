namespace SDSL.Native;

[ClassExport]
public static class NilClass
{
	public static NativeClass Class { get; } = NativeClass.CreatePrimitive("Nil", VariantType.Nil);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(NilClass), Class);
	}
}