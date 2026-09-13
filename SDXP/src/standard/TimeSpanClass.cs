namespace SDSL;

[ClassExport]
public static class TimeSpanClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("TimeSpan", VariantType.TimeSpan);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(TimeSpanClass), Class);
	}
}