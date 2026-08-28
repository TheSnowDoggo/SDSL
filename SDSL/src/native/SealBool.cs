using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public class SealBool
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.Global,
        "Bool"
    );
}