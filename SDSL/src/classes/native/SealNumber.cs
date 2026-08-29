using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public static class SealNumber
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Number",
        ValueType.Number
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