using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public static class SealNumber
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.Global,
        "Number",
        SealValueType.Nil
    );
    
    [FunctionExport("new(x: Any) -> Number")]
    public static SealValue New(ReadOnlySpan<SealValue> args)
    {
        SealValue value = args[0];

        return value.ValueType switch
        {
            SealValueType.Nil    => 0,
            SealValueType.Bool   => value.AsBool() ? 1 : 0,
            SealValueType.Number => value,
            SealValueType.String => 
                int.TryParse(value.AsString(), out int parsedValue)
                ? parsedValue
                : SealValue.Nil,
            _ => 0
        };
    }
}