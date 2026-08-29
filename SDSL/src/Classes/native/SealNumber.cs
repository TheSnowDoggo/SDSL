using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealNumber
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Number",
        ValueType.Number,
        true
    );
    
    [FunctionExport("new(x: Any) -> Number")]
    public static SealValue New(ReadOnlySpan<SealValue> args)
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
}