namespace SDSL;

[ClassExport]
public static class NumberClass
{
	public static VariantClass Class { get; } = new VariantClass("Number", VariantType.Number);

	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(NumberClass), Class);
	}
	
	[ConstantExport] public const double INF     = double.PositiveInfinity;
	[ConstantExport] public const double EPSILON = double.Epsilon;
	[ConstantExport] public const double MAX     = double.MaxValue;
	[ConstantExport] public const double MIN     = double.MinValue;
	[ConstantExport] public const double NAN     = double.NaN;
}