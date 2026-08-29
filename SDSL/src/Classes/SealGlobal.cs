using System.Text;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealGlobal
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        "global",
        "@global",
        ValueType.Object,
        true
    );
    
    [FunctionExport("to_string() -> String")]
    public static SealValue ToString(SealValue self, ReadOnlySpan<SealValue> args)
        => self.ToString();
    
    [FunctionExport("equals(other: Any) -> Bool")]
    public static SealValue Equals(SealValue self, ReadOnlySpan<SealValue> args)
        => self.Equals(args[0]);
    
    [FunctionExport("ref_equals(other: Any) -> Bool")]
    public static SealValue RefEquals(SealValue self, ReadOnlySpan<SealValue> args)
        => self.RefEquals(args[0]);
    
    [FunctionExport("to_bool() -> Bool")]
    public static SealValue ToBool(SealValue self, ReadOnlySpan<SealValue> args)
        => self.ToBool();

    [FunctionExport("range(start: Number, end: Number = ?, step: Number = ?) -> Range")]
    public static SealValue Range(ReadOnlySpan<SealValue> args) => args.Length switch
    {
        1 => new SealRange(SealRange.CreateRange(args[0].AsNumber())),
        2 => new SealRange(SealRange.CreateRange(args[0].AsNumber(), args[1].AsNumber())),
        3 => new SealRange(SealRange.CreateRange(args[0].AsNumber(), args[1].AsNumber(), args[2].AsNumber())),
        _ => throw new ArgumentException($"Expectd 1, 2, or 3 arguments, got {args.Length}.")
    };
    
    [FunctionExport("print(args..) -> Nil")]
    public static void Print(ReadOnlySpan<SealValue> args)
    {
        Console.Write(JoinArgs(args));
    }
    
    [FunctionExport("println(args..) -> Nil")]
    public static void Println(ReadOnlySpan<SealValue> args)
    {
        Console.WriteLine(JoinArgs(args));
    }
    
    [FunctionExport("read() -> Number")]
    public static SealValue Read(ReadOnlySpan<SealValue> args)
    {
        return Console.Read();
    }
    
    [FunctionExport("readln() -> String")]
    public static SealValue Readln(ReadOnlySpan<SealValue> args)
    {
        return Console.ReadLine() ?? string.Empty;
    }

    private static string JoinArgs(ReadOnlySpan<SealValue> args)
    {
        switch (args.Length)
        {
            case 0:
                return string.Empty;
            case 1:
                return args[0].ToString();
            default:
                var sb = new StringBuilder();
            
                for (int i = 0; i < args.Length; i++)
                    sb.Append(args[i]);
            
                return sb.ToString();
        }
    }
}