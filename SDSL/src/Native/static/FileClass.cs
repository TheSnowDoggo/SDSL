namespace SDSL.Native;

public static class FileClass
{
    public static NativeClass Class { get; } = NativeClass.InheritObject("File");

    [FunctionInfo("filepath")]
    [FunctionExport("String")]
    public static Variant read_lines(Variant[] args)
    {
        string[] lines = File.ReadAllLines(args[0].AsString());
        
        return new PackedStringArray(lines);
    }
    
    [FunctionInfo("filepath")]
    [FunctionExport("String")]
    public static Variant read_text(Variant[] args)
    {
        return File.ReadAllText(args[0].AsString());;
    }
    
    [FunctionInfo("filepath", "text")]
    [FunctionExport("String", "String")]
    public static void write_text(Variant[] args)
    {
        File.WriteAllText(args[0].AsString(), args[1].AsString());
    }
}