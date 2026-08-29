using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealBool
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Bool",
        ValueType.Bool,
        true
    );
}