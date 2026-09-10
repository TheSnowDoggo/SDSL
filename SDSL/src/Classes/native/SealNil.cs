using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealNil
{
    [ClassExport]
    public static readonly SealClass Class = SealClass.CreateGlobal("Nil", ValueType.Nil);
}