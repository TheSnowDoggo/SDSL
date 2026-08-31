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
    public static SealValue ToString(SealValue self, SealValue[] args)
        => self.ToString();
    
    [FunctionExport("equals(other: Any) -> Bool")]
    public static SealValue Equals(SealValue self, SealValue[] args)
        => self.Equals(args[0]);
    
    [FunctionExport("ref_equals(other: Any) -> Bool")]
    public static SealValue RefEquals(SealValue self, SealValue[] args)
        => self.RefEquals(args[0]);
    
    [FunctionExport("to_bool() -> Bool")]
    public static SealValue ToBool(SealValue self, SealValue[] args)
        => self.ToBool();

    [FunctionExport("range(start: Number, end: Number = ?, step: Number = ?) -> Range")]
    public static SealValue Range(SealValue[] args) => args.Length switch
    {
        1 => new SealRange(SealRange.CreateRange(args[0].AsNumber())),
        2 => new SealRange(SealRange.CreateRange(args[0].AsNumber(), args[1].AsNumber())),
        3 => new SealRange(SealRange.CreateRange(args[0].AsNumber(), args[1].AsNumber(), args[2].AsNumber())),
        _ => throw new ArgumentException($"Expectd 1, 2, or 3 arguments, got {args.Length}.")
    };
    
    [FunctionExport("print(args..) -> Nil")]
    public static void Print(SealValue[] args)
    {
        Console.Write(JoinArgs(args));
    }
    
    [FunctionExport("println(args..) -> Nil")]
    public static void Println(SealValue[] args)
    {
        Console.WriteLine(JoinArgs(args));
    }
    
    [FunctionExport("print_rich(format: String, args..) -> Nil")]
    public static void PrintRich(SealValue[] args)
    {
        PrintRich(args[0].AsString(), args);
    }
    
    [FunctionExport("println_rich(format: String, args..) -> Nil")]
    public static void PrintlnRich(SealValue[] args)
    {
        PrintRich(args[0].AsString(), args);
        Console.WriteLine();
    }
    
    [FunctionExport("read() -> Number")]
    public static SealValue Read(SealValue[] args)
    {
        return Console.Read();
    }
    
    [FunctionExport("readln() -> String")]
    public static SealValue Readln(SealValue[] args)
    {
        return Console.ReadLine() ?? string.Empty;
    }

    private static void PrintRich(string format, SealValue[] args)
    {
        PrintRich(SealString.Format(format, args));
    }

    private static void PrintRich(string s)
    {
        var fgStack = new Stack<ConsoleColor>();
        var bgStack = new Stack<ConsoleColor>();
        
        var sb = new StringBuilder();

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '<' && (i == 0 || s[i - 1] != '/'))
            {
                int close = s.IndexOf('>', i + 1);

                // No end tag is found or it is right after the open tag
                if (close == -1 || close == i + 1)
                {
                    sb.Append(s, i, s.Length - i);
                    break;
                }

                string key;
                string value;

                if (s[i + 1] == '/')
                {
                    key = s[(i + 2)..close];
                    value = null;
                }
                else
                {
                    int assign = s.IndexOf('=', i + 1, close - i - 1);

                    if (assign == -1)
                    {
                        key = s[(i + 1)..close];
                        value = nameof(ConsoleColor.Black);
                    }
                    else
                    {
                        key = s[(i + 1)..assign];
                        value = s[(assign + 1)..close];
                    }
                }

                int start = i;
                i = close;

                switch (key.Trim().ToLower())
                {
                case "fg":
                    // Close tag
                    if (value == null)
                    {
                        if (fgStack.TryPop(out ConsoleColor lastColor))
                        {
                            if (lastColor == Console.ForegroundColor)
                            {
                                continue;
                            }
                            
                            FlushBuilder(sb);
                            
                            Console.ForegroundColor = lastColor;
                            
                            continue;
                        }
                    }
                    // Open tag
                    else if (Enum.TryParse(value, true, out ConsoleColor nextColor))
                    {
                        fgStack.Push(Console.ForegroundColor);
                        
                        if (nextColor == Console.ForegroundColor)
                        {
                            continue;
                        }
                        
                        FlushBuilder(sb);

                        Console.ForegroundColor = nextColor;
                        
                        continue;
                    }
                    
                    break;
                case "bg":
                    // Close tag
                    if (value == null)
                    {
                        if (bgStack.TryPop(out ConsoleColor lastColor))
                        {
                            if (lastColor == Console.BackgroundColor)
                            {
                                continue;
                            }
                            
                            FlushBuilder(sb);
                            
                            Console.BackgroundColor = lastColor;
                            
                            continue;
                        }
                    }
                    // Open tag
                    else if (Enum.TryParse(value, true, out ConsoleColor nextColor))
                    {
                        bgStack.Push(Console.BackgroundColor);
                        
                        if (nextColor == Console.BackgroundColor)
                        {
                            continue;
                        }
                        
                        FlushBuilder(sb);

                        Console.BackgroundColor = nextColor;
                        
                        continue;
                    }
                    
                    break;
                }
                
                sb.Append(s, start, 1 + close - start);
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length != 0)
        {
            Console.Write(sb.ToString());
        }
    }

    private static void FlushBuilder(StringBuilder sb)
    {
        if (sb.Length == 0)
        {
            return;
        }
        
        Console.Write(sb.ToString());
        sb.Clear();
    }

    private static string JoinArgs(SealValue[] args)
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