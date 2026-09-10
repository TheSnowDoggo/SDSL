using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealBool
{
    public static readonly SealClass Class = SealClass.CreateGlobal("Bool", ValueType.Bool);
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        pAssembly.CreateClass(Class);
    }
}