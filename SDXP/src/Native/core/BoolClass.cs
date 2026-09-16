namespace SDSL.Native;

[ClassExport]
public static class BoolClass
{
	public static NativeClass Class { get; } = new NativeClass("Bool", VariantType.Bool);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(BoolClass), Class);
	}
}