using SDSL.Prototypes;

namespace SDSL;

[ClassExport]
public class SealFunction
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.Global,
        "Function"
    );
}