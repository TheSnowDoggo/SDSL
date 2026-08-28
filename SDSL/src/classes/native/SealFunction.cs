using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public static class SealFunction
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.Global,
        "Function",
        SealValueType.Function
    );
}