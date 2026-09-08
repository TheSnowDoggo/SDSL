using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealBool
{
    [ClassExport]
    public static readonly SealClass Class = SealClass.CreateGlobal("Bool", ValueType.Bool);
}