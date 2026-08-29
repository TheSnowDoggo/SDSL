using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealTimeSpan
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "TimeSpan",
        ValueType.TimeSpan,
        true
    );
}