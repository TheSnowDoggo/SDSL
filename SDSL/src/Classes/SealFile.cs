using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealFile
{
	[ClassExport]
	public static readonly SealClass Class = SealClass.CreateGlobal("File");

	[FunctionExport("read_lines(file_path: String)")]
	public static SealValue ReadLines(SealValue[] args)
	{
		string[] lines = File.ReadAllLines(args[0].AsString());

		return new PackedStringArray(lines);
	}
}