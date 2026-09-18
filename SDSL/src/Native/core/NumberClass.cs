namespace SDSL.Native;

public static class NumberClass
{
	public static NativeClass Class { get; } = NativeClass.CreatePrimitive("Number", VariantType.Number);

	[ConstantExport] public const double Inf     = double.PositiveInfinity;
	[ConstantExport] public const double Epsilon = double.Epsilon;
	[ConstantExport] public const double Max     = double.MaxValue;
	[ConstantExport] public const double Min     = double.MinValue;
	[ConstantExport] public const double NaN     = double.NaN;

	[FunctionInfo("value")]
	[FunctionExport("Any")]
	public static Variant parse(Variant[] args)
	{
		Variant value = args[0];

		return value.VariantType switch
		{
			VariantType.Bool   => value.AsBool() ? 1 : 0,
			VariantType.Number => value,
			VariantType.String => double.TryParse(
				value.AsString(), out double result) ? result : Variant.Nil,
			_ => Variant.Nil,
		};
	}
	
	[FunctionExport("Number")]
	public static Variant is_int(Variant[] args)
	{
		return double.IsInteger(args[0].AsDouble());
	}
	
	[FunctionExport("Number")]
	public static Variant is_positive_int(Variant[] args)
	{
		double x = args[0].AsDouble();
		return x >= 0 && double.IsInteger(x);
	}
    
	[FunctionExport("Number")]
	public static Variant is_even(Variant[] args)
	{
		return double.IsEvenInteger(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_odd(Variant[] args)
	{
		return double.IsOddInteger(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_nan(Variant[] args)
	{
		return double.IsNaN(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_ninf(Variant[] args)
	{
		return double.IsNegativeInfinity(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_pinf(Variant[] args)
	{
		return double.IsPositiveInfinity(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_normal(Variant[] args)
	{
		return double.IsNormal(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_sub_normal(Variant[] args)
	{
		return double.IsSubnormal(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_real(Variant[] args)
	{
		return double.IsRealNumber(args[0].AsDouble());
	}
    
	[FunctionExport("Number")]
	public static Variant is_pow2(Variant[] args)
	{
		return double.IsPow2(args[0].AsDouble());
	}
}