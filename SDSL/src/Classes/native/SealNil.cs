using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealNil
{
    public static readonly SealClass Class = SealClass.CreateGlobal("Nil", ValueType.Nil);

    public static void Generate(PrototypeAssembly pAssembly)
    {
        pAssembly.CreateClass(Class);
    }
}