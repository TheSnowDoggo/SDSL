using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealTimeSpan
{
    public static readonly SealClass Class = SealClass.CreateGlobal("TimeSpan", ValueType.TimeSpan);
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealTimeSpan), pAssembly, Class);
    }
    
    [SealFunctionInfo("days")]
    [SealFunctionExport("Number")]
    public static SealValue from_days(SealValue[] args)
    {
        return TimeSpan.FromDays(args[0].AsNumber());
    }

    [SealFunctionInfo("hours")]
    [SealFunctionExport("Number")]
    public static SealValue from_hours(SealValue[] args)
    {
        return TimeSpan.FromHours(args[0].AsNumber());
    }

    [SealFunctionInfo("minutes")]
    [SealFunctionExport("Number")]
    public static SealValue from_minutes(SealValue[] args)
    {
        return TimeSpan.FromMinutes(args[0].AsNumber());
    }

    [SealFunctionInfo("seconds")]
    [SealFunctionExport("Number")]
    public static SealValue from_seconds(SealValue[] args)
    {
        return TimeSpan.FromSeconds(args[0].AsNumber());
    }

    [SealFunctionInfo("milliseconds")]
    [SealFunctionExport("Number")]
    public static SealValue from_milliseconds(SealValue[] args)
    {
        return TimeSpan.FromMilliseconds(args[0].AsNumber());
    }

    [SealFunctionInfo("microseconds")]
    [SealFunctionExport("Number")]
    public static SealValue from_microseconds(SealValue[] args)
    {
        return TimeSpan.FromMicroseconds(args[0].AsNumber());
    }

    [SealFunctionExport]
    public static SealValue total_days(SealValue self)
    {
        return self.AsTimeSpan().TotalDays;
    }

    [SealFunctionExport]
    public static SealValue total_hours(SealValue self)
    {
        return self.AsTimeSpan().TotalHours;
    }

    [SealFunctionExport]
    public static SealValue total_minutes(SealValue self)
    {
        return self.AsTimeSpan().TotalMinutes;
    }

    [SealFunctionExport]
    public static SealValue total_seconds(SealValue self)
    {
        return self.AsTimeSpan().TotalSeconds;
    }

    [SealFunctionExport]
    public static SealValue total_milliseconds(SealValue self)
    {
        return self.AsTimeSpan().TotalMilliseconds;
    }

    [SealFunctionExport]
    public static SealValue total_microseconds(SealValue self)
    {
        return self.AsTimeSpan().TotalMicroseconds;
    }

    [SealFunctionExport]
    public static SealValue total_nanoseconds(SealValue self)
    {
        return self.AsTimeSpan().TotalNanoseconds;
    }

    [SealFunctionExport]
    public static SealValue days(SealValue self)
    {
        return self.AsTimeSpan().Days;
    }

    [SealFunctionExport]
    public static SealValue hours(SealValue self)
    {
        return self.AsTimeSpan().Hours;
    }

    [SealFunctionExport]
    public static SealValue minutes(SealValue self)
    {
        return self.AsTimeSpan().Minutes;
    }

    [SealFunctionExport]
    public static SealValue seconds(SealValue self)
    {
        return self.AsTimeSpan().Seconds;
    }

    [SealFunctionExport]
    public static SealValue milliseconds(SealValue self)
    {
        return self.AsTimeSpan().Milliseconds;
    }

    [SealFunctionExport]
    public static SealValue microsecond(SealValue self)
    {
        return self.AsTimeSpan().Microseconds;
    }

    [SealFunctionExport]
    public static SealValue nanoseconds(SealValue self)
    {
        return self.AsTimeSpan().Nanoseconds;
    }
}