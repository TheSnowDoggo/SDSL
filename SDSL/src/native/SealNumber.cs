using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public static class SealNumber
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.Global,
        "Number"
    );
    
    [FunctionExport("new(x: Any) -> Number")]
    public static SealValue New(ReadOnlySpan<SealValue> args)
    {
        SealValue value = args[0];

        return value.Class.GetTypeCatagory() switch
        {
            TypeCatagory.Nil    => 0,
            TypeCatagory.Bool   => value.AsBool() ? 1 : 0,
            TypeCatagory.Number => value,
            TypeCatagory.String => 
                int.TryParse(value.AsString(), out int parsedValue)
                ? parsedValue
                : SealValue.Nil,
            _ => 0
        };
    }
}