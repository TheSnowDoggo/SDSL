using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealColor
{
	public static readonly SealClass Class = SealClass.CreateGlobal("Color");

	public static void Generate(PrototypeAssembly pAssembly)
	{
		SealEnumFactory.Generate(typeof(ConsoleColor), pAssembly, Class);
	}
}