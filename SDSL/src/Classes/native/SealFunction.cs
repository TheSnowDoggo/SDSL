using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealFunction
{
    [ClassExport]
    public static readonly SealClass Class = SealClass.CreateGlobal("Function", ValueType.Function);
}