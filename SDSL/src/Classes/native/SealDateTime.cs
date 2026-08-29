using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealDateTime
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
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
}