using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class KeyCode
{
    public static readonly SealClass Class = SealClass.CreateGlobal("KeyCode");

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealEnumFactory.Generate(pAssembly, Class, typeof(ConsoleKey));
    }
}