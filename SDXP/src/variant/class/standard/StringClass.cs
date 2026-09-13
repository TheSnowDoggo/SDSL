namespace SDSL;

[ClassExport]
public static class StringClass
{
	public static VariantClass Class { get; } = new VariantClass("String", VariantType.String);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(StringClass), Class);
	}

	[FunctionExport]
	public static Variant to_string(Variant self)
	{
		return self;
	}
}