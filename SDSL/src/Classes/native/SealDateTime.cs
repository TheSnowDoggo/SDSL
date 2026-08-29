using System.Globalization;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealDateTime
{
    [ClassExport] public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "DateTime",
        ValueType.DateTime,
        true
    );

    [FunctionExport("new(date_time: String)")]
    public static SealValue New(ReadOnlySpan<SealValue> args)
    {
        return DateTime.TryParse(args[0].AsString(), out DateTime value)
            ? value
            : SealValue.Nil;
    }

    [FunctionExport("now() -> DateTime")]
    public static SealValue Now(ReadOnlySpan<SealValue> args)
        => DateTime.Now;
    
    [FunctionExport("utc_now() -> DateTime")]
    public static SealValue UtcNow(ReadOnlySpan<SealValue> args)
        => DateTime.UtcNow;

    [FunctionExport("day() -> Number")]
    public static SealValue Day(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Day;
    
    [FunctionExport("hour() -> Number")]
    public static SealValue Hour(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Hour;
    
    [FunctionExport("minute() -> Number")]
    public static SealValue Minute(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Minute;
    
    [FunctionExport("second() -> Number")]
    public static SealValue Second(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Second;
    
    [FunctionExport("millisecond() -> Number")]
    public static SealValue Millisecond(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Millisecond;
    
    [FunctionExport("microsecond() -> Number")]
    public static SealValue Microsecond(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Microsecond;
    
    [FunctionExport("nanosecond() -> Number")]
    public static SealValue Nanosecond(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsDateTime().Nanosecond;

    [FunctionExport("to_string(format: String = ?) -> String")]
    public static SealValue ToString(SealValue self, ReadOnlySpan<SealValue> args) => args.Length switch
    {
        0 => self.AsDateTime().ToString(CultureInfo.InvariantCulture),
        1 => self.AsDateTime().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };
}