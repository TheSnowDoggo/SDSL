using System.Text;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealGlobal
{
    [ClassExport]
    public static readonly SealClass Class = SealClass.CreateGlobal("@global");
    
    // <-- Overridable instance functions -->

    [FunctionExport("to_string() -> String")]
    public static SealValue _ToString(SealValue self, SealValue[] _)
    {
        return self.ToString();
    }

    [FunctionExport("equals(other: Any) -> Bool")]
    public static SealValue _Equals(SealValue self, SealValue[] args)
    {
        return self.Equals(args[0]);
    }

    [FunctionExport("ref_equals(other: Any) -> Bool")]
    public static SealValue _RefEquals(SealValue self, SealValue[] args)
    {
        return self.RefEquals(args[0]);
    }

    [FunctionExport("to_bool() -> Bool")]
    public static SealValue _ToBool(SealValue self, SealValue[] _)
    {
        return self.ToBool();
    }
    
    // <-- Global static functions -->
    
    [FunctionExport("range(start: Number, end: Number = ?, step: Number = ?) -> Range")]
    public static SealValue _Range(SealValue[] args) => args.Length switch
    {
        1 => SealRange.CreateRange(args[0].AsNumber()),
        2 => SealRange.CreateRange(args[0].AsNumber(), args[1].AsNumber()),
        3 => SealRange.CreateRange(args[0].AsNumber(), args[1].AsNumber(), args[2].AsNumber()),
        _ => throw new ArgumentException($"Expected 1, 2, or 3 arguments, got {args.Length}."),
    };
    
    [FunctionExport(("assert(condition: Bool, msg: String = ?)"))]
    public static void _Assert(SealValue[] args)
    {
        if (args[0].AsBool())
        {
            return;
        }
        
        string msg = args.Length > 1 ? args[1].AsString() : "Condition was false.";
            
        throw new RuntimeException(SourceLocation.Native, $"Assert failed: {msg}");
    }

    [FunctionExport("print(args..)")]
    public static void _Print(SealValue[] args)
    {
        Console.Write(JoinArgs(args));
    }
    
    [FunctionExport("print_line(args..)")]
    public static void _Printline(SealValue[] args)
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
                sb.Append(args[i]);
            
            return sb.ToString();
        }
    }
    
    [FunctionExport("printf(format: String, args..) -> Nil")]
    public static void _Printf(SealValue[] args)
    {
        Console.Write(SealString.Format(args[0].AsString(), args));
    }
    
    [FunctionExport("print_rich(s: String) -> Nil")]
    public static void _PrintRich(SealValue[] args)
    {
        PrintRich(args[0].AsString());
    }
    
    [FunctionExport("printf_rich(format: String, args..) -> Nil")]
    public static void _PrintfRich(SealValue[] args)
    {
        PrintRich(args[0].AsString(), args);
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
    
    [FunctionExport("read() -> Number")]
    public static SealValue Read(SealValue[] _)
    {
        return Console.Read();
    }
    
    [FunctionExport("read_key_info(intercept: Bool = ?) -> String")]
    public static SealValue ReadKeyInfo(SealValue[] args)
    {
        bool intercept = args.Length > 1 && args[0].AsBool();

        ConsoleKeyInfo cki = Console.ReadKey(intercept);

        return new SealMap()
        {
            { "key", (double)cki.Key },
            { "char", cki.KeyChar.ToString() },
        };
    }
    
    [FunctionExport("read_line() -> String")]
    public static SealValue Readline(SealValue[] _)
    {
        return Console.ReadLine() ?? string.Empty;
    }

    [FunctionExport("get_fg() -> String")]
    public static SealValue GetFg(SealValue[] _)
    {
        return Console.ForegroundColor.ToString();
    }

    [FunctionExport("set_fg(fg_color: String) -> Bool")]
    public static SealValue SetFg(SealValue[] args)
    {
        if (!Enum.TryParse(args[0].AsString(), true, out ConsoleColor color))
        {
            return false;
        }
        
        Console.ForegroundColor = color;
        
        return true;
    }
    
    [FunctionExport("get_bg() -> String")]
    public static SealValue GetBg(SealValue[] _)
    {
        return Console.BackgroundColor.ToString();
    }
    
    [FunctionExport("set_bg(bg_color: String) -> Bool")]
    public static SealValue SetBg(SealValue[] args)
    {
        if (!Enum.TryParse(args[0].AsString(), true, out ConsoleColor color))
        {
            return false;
        }
        
        Console.BackgroundColor = color;
        
        return true;
    }

    [FunctionExport("reset_color()")]
    public static void ResetColor(SealValue[] _)
    {
        Console.ResetColor();
    }

    [FunctionExport("get_cursor_left() -> Number")]
    public static SealValue GetCursorLeft(SealValue[] _)
    {
        return Console.CursorLeft;
    }
    
    [FunctionExport("get_cursor_top() -> Number")]
    public static SealValue GetCursorTop(SealValue[] _)
    {
        return Console.CursorTop;
    }
    
    [FunctionExport("set_cursor_left(left: Number)")]
    public static void SetCursorLeft(SealValue[] args)
    {
        Console.CursorLeft = args[0].AsInt32();
    }
    
    [FunctionExport("set_cursor_top(top: Number)")]
    public static void SetCursorTop(SealValue[] args)
    {
        Console.CursorTop = args[0].AsInt32();
    }

    [FunctionExport("set_cursor_visible(visible: Bool)")]
    public static void SetCursorVisible(SealValue[] args)
    {
        Console.CursorVisible = args[0].AsBool();
    }
    
    [FunctionExport("get_window_width() -> Number")]
    public static SealValue GetWindowWidth(SealValue[] _)
    {
        return Console.WindowWidth;
    }
    
    [FunctionExport("get_window_height() -> Number")]
    public static SealValue GetWindowHeight(SealValue[] _)
    {
        return Console.WindowHeight;
    }
    
    [FunctionExport("clear_console()")]
    public static void ClearConsole(SealValue[] _)
    {
        Console.Clear();
    }
}