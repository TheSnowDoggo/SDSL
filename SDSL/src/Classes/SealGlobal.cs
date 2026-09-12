using System.Text;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealGlobal
{
    public static readonly SealClass Class = SealClass.CreateGlobal("@global");
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealGlobal), pAssembly, Class);
    }
    
    // <-- Overridable instance functions -->

    [FunctionExport]
    public static SealValue to_string(SealValue self)
    {
        return self.ToString();
    }

    [FunctionExport("Any")]
    public static SealValue equals(SealValue self, SealValue[] args)
    {
        return self.Equals(args[0]);
    }

    [FunctionExport("Any")]
    public static SealValue ref_equals(SealValue self, SealValue[] args)
    {
        return self.RefEquals(args[0]);
    }

    [FunctionExport]
    public static SealValue to_bool(SealValue self)
    {
        return self.ToBool();
    }
    
    // <-- Global static functions -->
    
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public static SealValue type_of(SealValue[] args)
    {
        return args[0].Class.ToString();
    }
    
    [FunctionInfo("start", "end", "step")]
    [FunctionExport(SealNumber.Number, SealNumber.Number, SealNumber.Number, MinArgs = 1)]
    public static SealValue range(SealValue[] args) => args.Length switch
    {
        1 => SealRange.CreateRange(args[0].AsDouble()),
        2 => SealRange.CreateRange(args[0].AsDouble(), args[1].AsDouble()),
        3 => SealRange.CreateRange(args[0].AsDouble(), args[1].AsDouble(), args[2].AsDouble()),
        _ => throw new ArgumentException($"Expected 1, 2, or 3 arguments, got {args.Length}."),
    };
    
    [FunctionInfo("condition", "message")]
    [FunctionExport(SealBool.Bool, SealString.String, MinArgs = 1)]
    public static void assert(SealValue[] args)
    {
        if (args[0].AsBool())
        {
            return;
        }
        
        string msg = args.Length >= 2 ? args[1].AsString() : "Condition was false.";
            
        throw new RuntimeException(SourceLocation.Native, $"Assert failed: {msg}");
    }

    [FunctionInfo("message")]
    [FunctionExport(SealString.String, Name = "throw")]
    public static void _throw(SealValue[] args)
    {
        throw new RuntimeException(SourceLocation.Native, args[0].AsString());
    }

    [FunctionInfo("args..")]
    [FunctionExport(MaxArgs = -1)]
    public static void print(SealValue[] args)
    {
        Console.Write(JoinArgs(args));
    }
    
    [FunctionInfo("args..")]
    [FunctionExport(MaxArgs = -1)]
    public static void print_line(SealValue[] args)
    {
        Console.WriteLine(JoinArgs(args));
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
            {
                sb.Append(args[i]);
            }
            
            return sb.ToString();
        }
    }
    
    [FunctionInfo("format", "args..")]
    [FunctionExport(SealString.String, MaxArgs = -1)]
    public static void printf(SealValue[] args)
    {
        Console.Write(SealString.Format(args[0].AsString(), args));
    }
    
    [FunctionInfo("markup")]
    [FunctionExport(SealString.String)]
    public static void print_rich(SealValue[] args)
    {
        PrintRich(args[0].AsString());
    }
    
    [FunctionInfo("markup_format", "args..")]
    [FunctionExport(SealString.String, MaxArgs = -1)]
    public static void printf_rich(SealValue[] args)
    {
        PrintRich(SealString.Format(args[0].AsString(), args));
    }
    
    private static void PrintRich(string s)
    {
        var fgStack = new Stack<ConsoleColor>();
        var bgStack = new Stack<ConsoleColor>();
        
        var sb = new StringBuilder();

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            switch (c)
            {
            case '<':
                if (i + 1 < s.Length && s[i + 1] == '<')
                {
                    sb.Append('<');
                    i++;
                    continue;
                }
                
                int close = s.IndexOf('>', i + 1);

                // No end tag is found or it is right after the open tag
                if (close == -1 || close == i + 1)
                {
                    sb.Append(s, i, s.Length - i);
                    i = s.Length;
                    continue;
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
                
                break;
            case '>':
                if (i + 1 < s.Length && s[i + 1] == '>')
                {
                    i++;
                }

                sb.Append('>');
                
                break;
            default:
                sb.Append(c);
                break;
            }
        }

        if (sb.Length != 0)
        {
            Console.Write(sb.ToString());
        }
        
        return;
        
        static void FlushBuilder(StringBuilder sb)
        {
            if (sb.Length == 0)
            {
                return;
            }
        
            Console.Write(sb.ToString());
            sb.Clear();
        }
    }

    [FunctionExport]
    public static SealValue read()
    {
        return Console.Read();
    }
    
    [FunctionInfo("intercept")]
    [FunctionExport(SealBool.Bool, MinArgs = 0)]
    public static SealValue read_key_info(SealValue[] args)
    {
        bool intercept = args.Length >= 1 && args[0].AsBool();
        
        ConsoleKeyInfo cki = Console.ReadKey(intercept);

        return new SealMap()
        {
            { "key", (double)cki.Key },
            { "char", cki.KeyChar.ToString() },
        };
    }
    
    [FunctionExport]
    public static SealValue read_line()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    [FunctionExport]
    public static SealValue GetFg()
    {
        return (double)Console.ForegroundColor;
    }

    [FunctionInfo("color")]
    [FunctionExport(SealNumber.Number)]
    public static void set_fg(SealValue[] args)
    {
        Console.ForegroundColor = (ConsoleColor)args[0].AsInt32();
    }
    
    [FunctionExport]
    public static SealValue get_bg()
    {
        return (double)Console.BackgroundColor;
    }
    
    [FunctionInfo("color")]
    [FunctionExport(SealNumber.Number)]
    public static void set_bg(SealValue[] args)
    {
        Console.BackgroundColor = (ConsoleColor)args[0].AsInt32();
    }

    [FunctionExport]
    public static void reset_color()
    {
        Console.ResetColor();
    }

    [FunctionExport]
    public static SealValue get_cursor_left()
    {
        return Console.CursorLeft;
    }
    
    [FunctionExport]
    public static SealValue get_cursor_top()
    {
        return Console.CursorTop;
    }
    
    [FunctionInfo("left")]
    [FunctionExport(SealNumber.Number)]
    public static void set_cursor_left(SealValue[] args)
    {
        Console.CursorLeft = args[0].AsInt32();
    }
    
    [FunctionInfo("top")]
    [FunctionExport(SealNumber.Number)]
    public static void set_cursor_top(SealValue[] args)
    {
        Console.CursorTop = args[0].AsInt32();
    }

    [FunctionInfo("visible")]
    [FunctionExport(SealBool.Bool)]
    public static void set_cursor_visible(SealValue[] args)
    {
        Console.CursorVisible = args[0].AsBool();
    }
    
    [FunctionExport]
    public static SealValue get_window_width()
    {
        return Console.WindowWidth;
    }
    
    [FunctionExport]
    public static SealValue get_window_height()
    {
        return Console.WindowHeight;
    }
    
    [FunctionExport]
    public static void clear_console()
    {
        Console.Clear();
    }
}