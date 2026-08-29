using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public static class SealNil
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "Nil",
        ValueType.Nil
    );
}