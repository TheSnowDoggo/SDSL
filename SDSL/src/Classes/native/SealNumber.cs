using System.Globalization;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealNumber
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "Number",
        ValueType.Number,
        false
    );
    
    [FunctionExport("new(x: Any) -> Number")]
    public static SealValue New(SealValue[] args)
    {
        SealValue value = args[0];

        return value.ValueType switch
        {
            ValueType.Nil    => 0,
            ValueType.Bool   => value.AsBool() ? 1 : 0,
            ValueType.Number => value,
            ValueType.String => 
                int.TryParse(value.AsString(), out int parsedValue)
                ? parsedValue
                : SealValue.Nil,
            _ => 0
        };
    }

    [FunctionExport("to_string(format: String = ?)")]
    public static SealValue ToString(SealValue self, SealValue[] args) => args.Length switch
    {
        0 => self.AsNumber().ToString(CultureInfo.InvariantCulture),
        1 => self.AsNumber().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };
}