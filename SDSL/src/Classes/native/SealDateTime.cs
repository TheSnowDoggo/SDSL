using System.Globalization;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealDateTime
{
    [ClassExport] public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "DateTime",
        ValueType.DateTime,
        false
    );

    [FunctionExport("new(year: Number, month: Number, day: Number, hour: Number = ?, minute: Number = ?, second: Number = ?, millisecond: Number = ?, microsecond: Number = ?) -> DateTime")]
    public static SealValue New(SealValue[] args) => args.Length switch
    {
        3 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32()),
        4 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(), 0, 0),
        5 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(), args[4].AsInt32(), 0),
        6 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(), args[4].AsInt32(), args[5].AsInt32()),
        7 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(), args[4].AsInt32(), args[5].AsInt32(), args[6].AsInt32()),
        8 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(), args[4].AsInt32(), args[5].AsInt32(), args[6].AsInt32(), args[7].AsInt32()),
        _ => throw new ArgumentException($"Expected 3-9 arguments, got {args.Length}."),
    };

    [FunctionExport("parse(date_time: String)")]
    public static SealValue Parse(SealValue[] args)
    {
        return DateTime.TryParse(args[0].AsString(), out DateTime value)
            ? value
            : SealValue.Nil;
    }

    [FunctionExport("now() -> DateTime")]
    public static SealValue Now(SealValue[] args)
        => DateTime.Now;
    
    [FunctionExport("utc_now() -> DateTime")]
    public static SealValue UtcNow(SealValue[] args)
        => DateTime.UtcNow;

    [FunctionExport("day() -> Number")]
    public static SealValue Day(SealValue self, SealValue[] args)
        => self.AsDateTime().Day;
    
    [FunctionExport("hour() -> Number")]
    public static SealValue Hour(SealValue self, SealValue[] args)
        => self.AsDateTime().Hour;
    
    [FunctionExport("minute() -> Number")]
    public static SealValue Minute(SealValue self, SealValue[] args)
        => self.AsDateTime().Minute;
    
    [FunctionExport("second() -> Number")]
    public static SealValue Second(SealValue self, SealValue[] args)
        => self.AsDateTime().Second;
    
    [FunctionExport("millisecond() -> Number")]
    public static SealValue Millisecond(SealValue self, SealValue[] args)
        => self.AsDateTime().Millisecond;
    
    [FunctionExport("microsecond() -> Number")]
    public static SealValue Microsecond(SealValue self, SealValue[] args)
        => self.AsDateTime().Microsecond;
    
    [FunctionExport("nanosecond() -> Number")]
    public static SealValue Nanosecond(SealValue self, SealValue[] args)
        => self.AsDateTime().Nanosecond;

    [FunctionExport("to_string(format: String = ?) -> String")]
    public static SealValue ToString(SealValue self, SealValue[] args) => args.Length switch
    {
        0 => self.AsDateTime().ToString(CultureInfo.InvariantCulture),
        1 => self.AsDateTime().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };
}