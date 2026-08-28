using System.Text;
using SDSL.Prototypes;

namespace SDSL;

[ClassExport("global", "Console")]
public static class SealConsole
{
    [FunctionExport("print(args..) -> Nil")]
    public static void Print(ReadOnlySpan<SealValue> args)
    {
        Console.Write(JoinArgs(' ', args));
    }
    
    [FunctionExport("println(args..) -> Nil")]
    public static void Println(ReadOnlySpan<SealValue> args)
    {
        Console.WriteLine(JoinArgs(' ', args));
    }
    
    [FunctionExport("Read() -> Number")]
    public static SealValue Read(ReadOnlySpan<SealValue> args)
    {
        return Console.Read();
    }
    
    [FunctionExport("readln() -> String")]
    public static SealValue Readln(ReadOnlySpan<SealValue> args)
    {
        return Console.ReadLine() ?? string.Empty;
    }

    private static string JoinArgs(char seperator, ReadOnlySpan<SealValue> args)
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
            {
                sb.Append(args[i]);
                sb.Append(seperator);
            }
        
            return sb.ToString(0, sb.Length - 1);
        }
    }
}