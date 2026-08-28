using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public class SealNil
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.Global,
        "Nil"
    );
}