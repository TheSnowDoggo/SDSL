namespace SDSL.Native;

[ClassExport]
public static class NumberClass
{
	public static NativeClass Class { get; } = NativeClass.CreatePrimitive("Number", VariantType.Number);

	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(NumberClass), Class);
	}
	
	[ConstantExport] public const double INF     = double.PositiveInfinity;
	[ConstantExport] public const double EPSILON = double.Epsilon;
	[ConstantExport] public const double MAX     = double.MaxValue;
	[ConstantExport] public const double MIN     = double.MinValue;
	[ConstantExport] public const double NAN     = double.NaN;

	[FunctionInfo("x", "y")]
	[FunctionExport("Number", "Number", ReturnType = "Number")]
	public static Variant add(Variant[] args)
	{
		return args[0].AsDouble() + args[1].AsDouble();
	}
}