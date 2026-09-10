using System.Globalization;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealDateTime
{
    public static readonly SealClass Class = SealClass.CreateGlobal("DateTime", ValueType.DateTime);

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealDateTime), pAssembly, Class);
    }
    
    [SealConstructor]
    [SealFunctionInfo("year", "month", "day", "hour", "minute", "second", "millisecond", "microsecond")]
    [SealFunctionExport("Number", "Number", "Number", "Number", "Number", "Number", "Number", "Number")]
    public static SealValue _new(SealValue[] args)
    {
        return args.Length switch
        {
            3 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32()),
            4 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(), 0, 0),
            5 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(),
                args[4].AsInt32(), 0),
            6 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(),
                args[4].AsInt32(), args[5].AsInt32()),
            7 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(),
                args[4].AsInt32(), args[5].AsInt32(), args[6].AsInt32()),
            8 => new DateTime(args[0].AsInt32(), args[1].AsInt32(), args[2].AsInt32(), args[3].AsInt32(),
                args[4].AsInt32(), args[5].AsInt32(), args[6].AsInt32(), args[7].AsInt32()),
            _ => throw new ArgumentException($"Expected 3-9 arguments, got {args.Length}."),
        };
    }

    [SealFunctionInfo("s")]
    [SealFunctionExport("String")]
    public static SealValue parse(SealValue[] args)
    {
        return DateTime.TryParse(args[0].AsString(), out DateTime value)
            ? value
            : SealValue.Nil;
    }

    [SealFunctionExport]
    public static SealValue now()
    {
        return DateTime.Now;
    }

    [SealFunctionExport]
    public static SealValue utc_now()
    {
        return DateTime.UtcNow;
    }

    [SealFunctionExport]
    public static SealValue day(SealValue self)
    {
        return self.AsDateTime().Day;
    }

    [SealFunctionExport]
    public static SealValue hour(SealValue self)
    {
        return self.AsDateTime().Hour;
    }

    [SealFunctionExport]
    public static SealValue minute(SealValue self)
    {
        return self.AsDateTime().Minute;
    }

    [SealFunctionExport]
    public static SealValue second(SealValue self)
    {
        return self.AsDateTime().Second;
    }

    [SealFunctionExport]
    public static SealValue millisecond(SealValue self)
    {
        return self.AsDateTime().Millisecond;
    }

    [SealFunctionExport]
    public static SealValue microsecond(SealValue self)
    {
        return self.AsDateTime().Microsecond;
    }

    [SealFunctionExport]
    public static SealValue nanosecond(SealValue self)
    {
        return self.AsDateTime().Nanosecond;
    }

    [SealFunctionExport("String", MinArgs = 0)]
    public static SealValue to_string(SealValue self, SealValue[] args) => args.Length switch
    {
        0 => self.AsDateTime().ToString(CultureInfo.InvariantCulture),
        1 => self.AsDateTime().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}."),
    };
}