using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealTimeSpan
{
    public static readonly SealClass Class = SealClass.CreateGlobal("TimeSpan", SealValueType.TimeSpan);
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealTimeSpan), pAssembly, Class);
    }
    
    [FunctionInfo("days")]
    [FunctionExport(SealNumber.Number)]
    public static SealValue from_days(SealValue[] args)
    {
        return TimeSpan.FromDays(args[0].AsDouble());
    }

    [FunctionInfo("hours")]
    [FunctionExport(SealNumber.Number)]
    public static SealValue from_hours(SealValue[] args)
    {
        return TimeSpan.FromHours(args[0].AsDouble());
    }

    [FunctionInfo("minutes")]
    [FunctionExport(SealNumber.Number)]
    public static SealValue from_minutes(SealValue[] args)
    {
        return TimeSpan.FromMinutes(args[0].AsDouble());
    }

    [FunctionInfo("seconds")]
    [FunctionExport(SealNumber.Number)]
    public static SealValue from_seconds(SealValue[] args)
    {
        return TimeSpan.FromSeconds(args[0].AsDouble());
    }

    [FunctionInfo("milliseconds")]
    [FunctionExport(SealNumber.Number)]
    public static SealValue from_milliseconds(SealValue[] args)
    {
        return TimeSpan.FromMilliseconds(args[0].AsDouble());
    }

    [FunctionInfo("microseconds")]
    [FunctionExport(SealNumber.Number)]
    public static SealValue from_microseconds(SealValue[] args)
    {
        return TimeSpan.FromMicroseconds(args[0].AsDouble());
    }

    [FunctionExport]
    public static SealValue total_days(SealValue self)
    {
        return self.AsTimeSpan().TotalDays;
    }

    [FunctionExport]
    public static SealValue total_hours(SealValue self)
    {
        return self.AsTimeSpan().TotalHours;
    }

    [FunctionExport]
    public static SealValue total_minutes(SealValue self)
    {
        return self.AsTimeSpan().TotalMinutes;
    }

    [FunctionExport]
    public static SealValue total_seconds(SealValue self)
    {
        return self.AsTimeSpan().TotalSeconds;
    }

    [FunctionExport]
    public static SealValue total_milliseconds(SealValue self)
    {
        return self.AsTimeSpan().TotalMilliseconds;
    }

    [FunctionExport]
    public static SealValue total_microseconds(SealValue self)
    {
        return self.AsTimeSpan().TotalMicroseconds;
    }

    [FunctionExport]
    public static SealValue total_nanoseconds(SealValue self)
    {
        return self.AsTimeSpan().TotalNanoseconds;
    }

    [FunctionExport]
    public static SealValue days(SealValue self)
    {
        return self.AsTimeSpan().Days;
    }

    [FunctionExport]
    public static SealValue hours(SealValue self)
    {
        return self.AsTimeSpan().Hours;
    }

    [FunctionExport]
    public static SealValue minutes(SealValue self)
    {
        return self.AsTimeSpan().Minutes;
    }

    [FunctionExport]
    public static SealValue seconds(SealValue self)
    {
        return self.AsTimeSpan().Seconds;
    }

    [FunctionExport]
    public static SealValue milliseconds(SealValue self)
    {
        return self.AsTimeSpan().Milliseconds;
    }

    [FunctionExport]
    public static SealValue microsecond(SealValue self)
    {
        return self.AsTimeSpan().Microseconds;
    }

    [FunctionExport]
    public static SealValue nanoseconds(SealValue self)
    {
        return self.AsTimeSpan().Nanoseconds;
    }
}