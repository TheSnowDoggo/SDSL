using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealBool
{
    public static readonly SealClass Class = SealClass.CreateGlobal("Bool", SealValueType.Bool);
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        pAssembly.CreateClass(Class);
    }
}