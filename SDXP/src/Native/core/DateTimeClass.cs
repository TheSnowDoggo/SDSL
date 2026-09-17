using System.Globalization;

namespace SDSL.Native;

[ClassExport]
public static class DateTimeClass
{
	public static NativeClass Class { get; } = NativeClass.CreatePrimitive("DateTime", VariantType.DateTime);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		NativeClassFactory.GenerateClass(variantAssembly, typeof(DateTimeClass), Class);
	}
	
    [FunctionInfo("year", "month", "day", "hour", "minute", "second", "millisecond", "microsecond")]
    [ConstructorExport("Number", "Number", "Number", "Number", "Number", "Number", "Number", "Number")]
    public static Variant _new(Variant[] a)
    {
        return a.Length switch
        {
            3 => new DateTime(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32()),
            4 => new DateTime(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), 0, 0),
            5 => new DateTime(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32(), 0),
            6 => new DateTime(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32(), a[5].AsInt32()),
            7 => new DateTime(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32(), a[5].AsInt32(), a[6].AsInt32()),
            8 => new DateTime(a[0].AsInt32(), a[1].AsInt32(), a[2].AsInt32(), a[3].AsInt32(), a[4].AsInt32(), a[5].AsInt32(), a[6].AsInt32(), a[7].AsInt32()),
            _ => throw new ArgumentException($"Expected 3-9 arguments, got {a.Length}."),
        };
    }

    [FunctionInfo("s")]
    [FunctionExport("String")]
    public static Variant parse(Variant[] args)
    {
        return DateTime.TryParse(args[0].AsString(), out DateTime value) ? value : Variant.Nil;
    }

    [PropertyExport]
    public static Variant now => DateTime.Now;

    [PropertyExport]
    public static Variant utc_now => DateTime.UtcNow;

    [GetterFunctionExport]
    public static Variant day(Variant self)
    {
        return self.AsDateTime().Day;
    }

    [GetterFunctionExport]
    public static Variant hour(Variant self)
    {
        return self.AsDateTime().Hour;
    }

    [GetterFunctionExport]
    public static Variant minute(Variant self)
    {
        return self.AsDateTime().Minute;
    }

    [GetterFunctionExport]
    public static Variant second(Variant self)
    {
        return self.AsDateTime().Second;
    }

    [GetterFunctionExport]
    public static Variant millisecond(Variant self)
    {
        return self.AsDateTime().Millisecond;
    }

    [GetterFunctionExport]
    public static Variant microsecond(Variant self)
    {
        return self.AsDateTime().Microsecond;
    }

    [GetterFunctionExport]
    public static Variant nanosecond(Variant self)
    {
        return self.AsDateTime().Nanosecond;
    }

    [FunctionExport("String", MinArgs = 0)]
    public static Variant to_string(Variant self, Variant[] args) => args.Length switch
    {
        0 => self.AsDateTime().ToString(CultureInfo.InvariantCulture),
        1 => self.AsDateTime().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}."),
    };
}