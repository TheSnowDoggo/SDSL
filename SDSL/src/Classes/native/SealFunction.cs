using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealFunction
{
    public static readonly SealClass Class = SealClass.CreateGlobal("Function", ValueType.Function);
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        pAssembly.CreateClass(Class);
    }
}