using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealNil
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Nil",
        ValueType.Nil,
        true
    );
}