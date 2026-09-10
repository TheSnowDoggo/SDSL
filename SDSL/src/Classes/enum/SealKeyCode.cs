using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealKeyCode
{
    public static readonly SealClass Class = SealClass.CreateGlobal("KeyCode");

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealEnumFactory.Generate(typeof(ConsoleKey), pAssembly, Class);
    }
}