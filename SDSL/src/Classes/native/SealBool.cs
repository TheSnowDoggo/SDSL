using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealBool
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Bool",
        ValueType.Bool,
        true
    );
}