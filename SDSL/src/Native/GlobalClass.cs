using System.Text;

namespace SDSL.Native;

[ClassExport]
public static class GlobalClass
{
    public static NativeClass Class { get; } = NativeClass.InheritObject("Global");
    
    public static void Generate(VariantAssembly assembly)
    {
        NativeClassFactory.GenerateClass(assembly, typeof(GlobalClass), Class);
    }
    
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public static Variant type_of(Variant[] args)
    {
        return args[0].Class.ToString();
    }
    
    [FunctionInfo("start", "end", "step")]
    [FunctionExport("Number", "Number", "Number", MinArgs = 1)]
    public static Variant range(Variant[] args) => args.Length switch
    {
        1 => NativeRange.CreateRange(args[0].AsDouble()),
        2 => NativeRange.CreateRange(args[0].AsDouble(), args[1].AsDouble()),
        3 => NativeRange.CreateRange(args[0].AsDouble(), args[1].AsDouble(), args[2].AsDouble()),
        _ => throw new ArgumentException($"Expected 1, 2, or 3 arguments, got {args.Length}."),
    };
    
    [FunctionInfo("condition", "message")]
    [FunctionExport("Bool", "String", MinArgs = 1)]
    public static void assert(Variant[] args)
    {
        if (args[0].AsBool())
        {
            return;
        }
        
        string msg = args.Length >= 2 ? args[1].AsString() : "Condition was false.";
            
        throw new RuntimeException(SourceLocation.Native, $"Assert failed: {msg}");
    }

    [FunctionInfo("message")]
    [FunctionExport("String", Name = "throw")]
    public static void _throw(Variant[] args)
    {
        throw new RuntimeException(SourceLocation.Native, args[0].AsString());
    }

    [FunctionInfo("args..")]
    [FunctionExport(MaxArgs = -1)]
    public static void print(Variant[] args)
    {
        Console.Write(JoinArgs(args));
    }
    
    [FunctionInfo("args..")]
    [FunctionExport(MaxArgs = -1)]
    public static void printl(Variant[] args)
    {
        Console.WriteLine(JoinArgs(args));
    }
    
    private static string JoinArgs(Variant[] args)
    {
        switch (args.Length)
        {
        case 0:
            return string.Empty;
        case 1:
            return args[0].ToStringVolatile();
        default:
            var sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].ToStringVolatile());
            }
            
            return sb.ToString();
        }
    }
    
    [FunctionInfo("format", "args..")]
    [FunctionExport("String", MaxArgs = -1)]
    public static void printf(Variant[] args)
    {
        Console.Write(StringClass.Format(args[0].AsString(), args));
    }
    
    [FunctionInfo("markup")]
    [FunctionExport("String")]
    public static void print_rich(Variant[] args)
    {
        PrintRich(args[0].AsString());
    }
    
    [FunctionInfo("markup_format", "args..")]
    [FunctionExport("String", MaxArgs = -1)]
    public static void printf_rich(Variant[] args)
    {
        PrintRich(StringClass.Format(args[0].AsString(), args));
    }
    
    private static void PrintRich(string s)
    {
        var fgStack = new Stack<ConsoleColor>();
        var bgStack = new Stack<ConsoleColor>();

        ConsoleColor initialFg = Console.ForegroundColor;
        ConsoleColor initialBg = Console.BackgroundColor;
        
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

        Console.ForegroundColor = initialFg;
        Console.BackgroundColor = initialBg;
        
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
    public static Variant read()
    {
        return Console.Read();
    }
    
    [FunctionInfo("intercept")]
    [FunctionExport("Bool", MinArgs = 0)]
    public static Variant read_key_info(Variant[] args)
    {
        bool intercept = args.Length >= 1 && args[0].AsBool();
        
        ConsoleKeyInfo cki = Console.ReadKey(intercept);

        return new NativeMap()
        {
            { "key", (double)cki.Key },
            { "char", cki.KeyChar.ToString() },
        };
    }
    
    [FunctionExport]
    public static Variant readl()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    [FunctionExport]
    public static Variant get_fg()
    {
        return (double)Console.ForegroundColor;
    }

    [FunctionInfo("color")]
    [FunctionExport("Number")]
    public static void set_fg(Variant[] args)
    {
        Console.ForegroundColor = (ConsoleColor)args[0].AsInt32();
    }
    
    [FunctionExport]
    public static Variant get_bg()
    {
        return (double)Console.BackgroundColor;
    }
    
    [FunctionInfo("color")]
    [FunctionExport("Number")]
    public static void set_bg(Variant[] args)
    {
        Console.BackgroundColor = (ConsoleColor)args[0].AsInt32();
    }

    [FunctionExport]
    public static void reset_color()
    {
        Console.ResetColor();
    }

    [FunctionExport]
    public static Variant get_cursor_left()
    {
        return Console.CursorLeft;
    }
    
    [FunctionExport]
    public static Variant get_cursor_top()
    {
        return Console.CursorTop;
    }
    
    [FunctionInfo("left")]
    [FunctionExport("Number")]
    public static void set_cursor_left(Variant[] args)
    {
        Console.CursorLeft = args[0].AsInt32();
    }
    
    [FunctionInfo("top")]
    [FunctionExport("Number")]
    public static void set_cursor_top(Variant[] args)
    {
        Console.CursorTop = args[0].AsInt32();
    }

    [FunctionInfo("visible")]
    [FunctionExport("Bool")]
    public static void set_cursor_visible(Variant[] args)
    {
        Console.CursorVisible = args[0].AsBool();
    }
    
    [FunctionExport]
    public static Variant get_window_width()
    {
        return Console.WindowWidth;
    }
    
    [FunctionExport]
    public static Variant get_window_height()
    {
        return Console.WindowHeight;
    }
    
    [FunctionExport]
    public static void clear_console()
    {
        Console.Clear();
    }
}