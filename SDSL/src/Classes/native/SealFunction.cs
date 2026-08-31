using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealFunction
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "Function",
        ValueType.Function,
        true
    );
}