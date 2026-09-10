using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealFile
{
	public static readonly SealClass Class = SealClass.CreateGlobal("File");

	public static void Generate(PrototypeAssembly pAssembly)
	{
		SealClassFactory.Generate(typeof(SealFile), pAssembly, Class);
	}
	
	[SealFunctionInfo("file_path")]
	[SealFunctionExport("String")]
	public static SealValue ReadLines(SealValue[] args)
	{
		string[] lines = File.ReadAllLines(args[0].AsString());

		return new PackedStringArray(lines);
	}
}