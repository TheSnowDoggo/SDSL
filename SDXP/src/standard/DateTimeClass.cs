namespace SDSL;

[ClassExport]
public static class DateTimeClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("DateTime", VariantType.DateTime);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(DateTimeClass), Class);
	}
}