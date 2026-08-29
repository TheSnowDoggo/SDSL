using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealFunction
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Function",
        ValueType.Function,
        true
    );
}