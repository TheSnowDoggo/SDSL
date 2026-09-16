namespace SDSL.Native;

[ClassExport]
public static class ColorClass
{
	public static NativeClass Class { get; } = new NativeClass("Color", VariantType.Nil);

	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateEnum(variantAssembly, typeof(ConsoleColor), Class);
	}
}