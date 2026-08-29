using System.Globalization;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealTimeSpan
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "TimeSpan",
        ValueType.TimeSpan,
        true
    );
    
    [FunctionExport("total_days() -> Number")]
    public static SealValue TotalDays(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalDays;
    
    [FunctionExport("total_hours() -> Number")]
    public static SealValue TotalHours(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalHours;
    
    [FunctionExport("total_minutes() -> Number")]
    public static SealValue TotalMinutes(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalMinutes;

    [FunctionExport("total_seconds() -> Number")]
    public static SealValue TotalSeconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalSeconds;
    
    [FunctionExport("total_milliseconds() -> Number")]
    public static SealValue TotalMilliseconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalMilliseconds;
    
    [FunctionExport("total_microseconds() -> Number")]
    public static SealValue TotalMicroseconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalMicroseconds;
    
    [FunctionExport("total_nanoseconds() -> Number")]
    public static SealValue TotalNanoseconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().TotalNanoseconds;
    
    [FunctionExport("days() -> Number")]
    public static SealValue Days(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Days;
    
    [FunctionExport("hours() -> Number")]
    public static SealValue Hour(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Hours;
    
    [FunctionExport("minutes() -> Number")]
    public static SealValue Minutes(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Minutes;
    
    [FunctionExport("seconds() -> Number")]
    public static SealValue Seconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Seconds;
    
    [FunctionExport("milliseconds() -> Number")]
    public static SealValue Milliseconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Milliseconds;
    
    [FunctionExport("microseconds() -> Number")]
    public static SealValue Microsecond(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Microseconds;
    
    [FunctionExport("nanoseconds() -> Number")]
    public static SealValue Nanoseconds(SealValue self, SealValue[] args)
        => self.AsTimeSpan().Nanoseconds;
    
    [FunctionExport("from_days(days: Number) -> TimeSpan")]
    public static SealValue FromDays(SealValue[] args)
        => TimeSpan.FromDays(args[0].AsNumber());
    
    [FunctionExport("from_hours(hours: Number) -> TimeSpan")]
    public static SealValue FromHours(SealValue[] args)
        => TimeSpan.FromHours(args[0].AsNumber());
    
    [FunctionExport("from_minutes(minutes: Number) -> TimeSpan")]
    public static SealValue FromMinutes(SealValue[] args)
        => TimeSpan.FromMinutes(args[0].AsNumber());
    
    [FunctionExport("from_seconds(seconds: Number) -> TimeSpan")]
    public static SealValue FromSeconds(SealValue[] args)
        => TimeSpan.FromSeconds(args[0].AsNumber());
    
    [FunctionExport("from_milliseconds(milliseconds: Number) -> TimeSpan")]
    public static SealValue FromMilliseconds(SealValue[] args)
        => TimeSpan.FromMilliseconds(args[0].AsNumber());
    
    [FunctionExport("from_microseconds(milliseconds: Number) -> TimeSpan")]
    public static SealValue FromMicroseconds(SealValue[] args)
        => TimeSpan.FromMicroseconds(args[0].AsNumber());
}