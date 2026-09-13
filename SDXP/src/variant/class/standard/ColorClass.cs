namespace SDSL;

[ClassExport]
public static class ColorClass
{
	public static VariantClass Class { get; } = new VariantClass("Color", VariantType.Nil);

	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateEnum(variantAssembly, typeof(ConsoleColor), Class);
	}
}