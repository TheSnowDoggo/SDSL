namespace SDSL.Native;

[ClassExport]
public static class TimeSpanClass
{
	public static NativeClass Class { get; } = new NativeClass("TimeSpan", VariantType.TimeSpan);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(TimeSpanClass), Class);
	}
	
	[FunctionInfo("days")]
    [FunctionExport("Number")]
    public static Variant from_days(Variant[] args)
    {
        return TimeSpan.FromDays(args[0].AsDouble());
    }

    [FunctionInfo("hours")]
    [FunctionExport("Number")]
    public static Variant from_hours(Variant[] args)
    {
        return TimeSpan.FromHours(args[0].AsDouble());
    }

    [FunctionInfo("minutes")]
    [FunctionExport("Number")]
    public static Variant from_minutes(Variant[] args)
    {
        return TimeSpan.FromMinutes(args[0].AsDouble());
    }

    [FunctionInfo("seconds")]
    [FunctionExport("Number")]
    public static Variant from_seconds(Variant[] args)
    {
        return TimeSpan.FromSeconds(args[0].AsDouble());
    }

    [FunctionInfo("milliseconds")]
    [FunctionExport("Number")]
    public static Variant from_milliseconds(Variant[] args)
    {
        return TimeSpan.FromMilliseconds(args[0].AsDouble());
    }

    [FunctionInfo("microseconds")]
    [FunctionExport("Number")]
    public static Variant from_microseconds(Variant[] args)
    {
        return TimeSpan.FromMicroseconds(args[0].AsDouble());
    }

    [GetterFunctionExport]
    public static Variant total_days(Variant self)
    {
        return self.AsTimeSpan().TotalDays;
    }

    [GetterFunctionExport]
    public static Variant total_hours(Variant self)
    {
        return self.AsTimeSpan().TotalHours;
    }

    [GetterFunctionExport]
    public static Variant total_minutes(Variant self)
    {
        return self.AsTimeSpan().TotalMinutes;
    }

    [GetterFunctionExport]
    public static Variant total_seconds(Variant self)
    {
        return self.AsTimeSpan().TotalSeconds;
    }

    [GetterFunctionExport]
    public static Variant total_milliseconds(Variant self)
    {
        return self.AsTimeSpan().TotalMilliseconds;
    }

    [GetterFunctionExport]
    public static Variant total_microseconds(Variant self)
    {
        return self.AsTimeSpan().TotalMicroseconds;
    }

    [GetterFunctionExport]
    public static Variant total_nanoseconds(Variant self)
    {
        return self.AsTimeSpan().TotalNanoseconds;
    }

    [GetterFunctionExport]
    public static Variant days(Variant self)
    {
        return self.AsTimeSpan().Days;
    }

    [GetterFunctionExport]
    public static Variant hours(Variant self)
    {
        return self.AsTimeSpan().Hours;
    }

    [GetterFunctionExport]
    public static Variant minutes(Variant self)
    {
        return self.AsTimeSpan().Minutes;
    }

    [GetterFunctionExport]
    public static Variant seconds(Variant self)
    {
        return self.AsTimeSpan().Seconds;
    }

    [GetterFunctionExport]
    public static Variant milliseconds(Variant self)
    {
        return self.AsTimeSpan().Milliseconds;
    }

    [GetterFunctionExport]
    public static Variant microsecond(Variant self)
    {
        return self.AsTimeSpan().Microseconds;
    }

    [GetterFunctionExport]
    public static Variant nanoseconds(Variant self)
    {
        return self.AsTimeSpan().Nanoseconds;
    }
}