namespace SDSL;

[ClassExport]
public static class ColorClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("Color", VariantType.Nil);

	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateEnum(variantAssembly, typeof(ConsoleColor), Class);
	}
}