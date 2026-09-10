using System.Text;
using SDSL.Prototypes;

namespace SDSL.Classes;

[NativeClass]
public static class SealString
{
    [ClassExport]
    public static readonly SealClass Class = SealClass.CreateGlobal("String", ValueType.String);
    
    [FunctionExport("new(x: Any) -> String")]
    public static SealValue New(SealValue[] args)
        => args[0].ToString();
    
    [FunctionExport("size() -> Number")]
    public static SealValue Size(SealValue self, SealValue[] _)
        => self.AsString().Length;

    [FunctionExport("_get(index: Number) -> String")]
    public static SealValue _Getter(SealValue self, SealValue[] args)
        => self.AsString()[args[0].AsInt32()].ToString();
    
    [FunctionExport("trim() -> String")]
    public static SealValue Trim(SealValue self, SealValue[] _)
        => self.AsString().Trim();
    
    [FunctionExport("trim_start() -> String")]
    public static SealValue TrimStart(SealValue self, SealValue[] _)
        => self.AsString().TrimStart();
    
    [FunctionExport("trim_end() -> String")]
    public static SealValue TrimEnd(SealValue self, SealValue[] _)
        => self.AsString().TrimEnd();
    
    [FunctionExport("to_lower() -> String")]
    public static SealValue ToLower(SealValue self, SealValue[] _)
        => self.AsString().ToLowerInvariant();
    
    [FunctionExport("to_upper() -> String")]
    public static SealValue ToUpper(SealValue self, SealValue[] _)
        => self.AsString().ToUpperInvariant();
    
    [FunctionExport("to_snakecase() -> String")]
    public static SealValue ToSnake(SealValue self, SealValue[] _)
        => self.AsString().ToSnakeCase();

    [FunctionExport("get_code(index: Number = ?)")]
    public static SealValue ToCharCode(SealValue self, SealValue[] args)
    {
        string s = self.AsString();

        return args.Length switch
        {
            0 => s.Length > 0 ? (double)s[0] : SealValue.Nil,
            1 => ToCharCode(s, args[0].AsInt32()),
            _ => throw new ArgumentException($"Expected 0 or 1 arguments, got {args.Length}."),
        };
    }
    
    [FunctionExport("from_code(code: Number)")]
    public static SealValue FromCharCode(SealValue[] args)
    {
        int code = args[0].AsInt32();

        if (code is < 0 or >= char.MaxValue)
        {
            return SealValue.Nil;
        }

        return ((char)code).ToString();
    }

    private static SealValue ToCharCode(string s, int index)
    {
        if (index < 0 || index >= s.Length)
        {
            return SealValue.Nil;
        }

        return (double)s[index];
    }

    [FunctionExport("has(s: String) -> Bool")]
    public static SealValue Has(SealValue self, SealValue[] args)
        => self.AsString().Contains(args[0].AsString());

    [FunctionExport("index_of(s: String, start_index: Number = ?, count: Number = ?) -> Number")]
    public static SealValue IndexOf(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        string value = args[0].AsString();
        
        return args.Length switch
        {
            1 => s.IndexOf(value, StringComparison.InvariantCulture),
            2 => IndexOf(s, value, (int)args[1].AsNumber()),
            3 => IndexOf(s, value, (int)args[1].AsNumber(), (int)args[2].AsNumber()),
            _ => throw new ArgumentException($"Expected 1, 2, or 3 arguments, got {args.Length}."),
        };
    }

    private static SealValue IndexOf(string s, string value, int startIndex)
    {
        if (startIndex < 0 || startIndex >= s.Length)
        {
            return -1;
        }

        return s.IndexOf(value, startIndex, StringComparison.InvariantCulture);
    }
    
    private static SealValue IndexOf(string s, string value, int startIndex, int count)
    {
        if (startIndex < 0 || count < 0
            || startIndex >= s.Length || startIndex + count > s.Length)
        {
            return -1;
        }

        return s.IndexOf(value, startIndex, count, StringComparison.InvariantCulture);
    }
    
    [FunctionExport("replace(old_str: String, new_str: String) -> String")]
    public static SealValue Replace(SealValue self, SealValue[] args)
        => self.AsString().Replace(args[0].AsString(), args[1].AsString());

    [FunctionExport("sub_string(start: Number, count: Number = ?) -> String")]
    public static SealValue SubString(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        
        int start = (int)args[0].AsNumber();

        if (start >= s.Length)
        {
            return string.Empty;
        }

        return args.Length switch
        {
            1 => SubString(s, start),
            2 => SubString(s, start, args[1].AsInt32()),
            _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}."),
        };
    }

    private static SealValue SubString(string s, int start)
    {
        return s[Math.Max(start, 0)..];
    }
    
    private static SealValue SubString(string s, int start, int count)
    {
        int end = Math.Min(start + count, s.Length);
        
        return s[Math.Max(start, 0)..end];
    }
    
    [FunctionExport("is_empty() -> Bool")]
    public static SealValue IsEmpty(SealValue self, SealValue[] _)
        => string.IsNullOrEmpty(self.AsString());

    [FunctionExport("is_whitespace() -> Bool")]
    public static SealValue IsWhiteSpace(SealValue self, SealValue[] _)
        => string.IsNullOrWhiteSpace(self.AsString());

    [FunctionExport("pad_right(width: Number, pad: String = ?) -> String")]
    public static SealValue PadRight(SealValue self, SealValue[] args)
    {
        GetPaddingArgs(args, out int width, out string pad);

        return PadRight(self.AsString(), width, pad);
    }
    
    [FunctionExport("pad_left(width: Number, pad: String = ?) -> String")]
    public static SealValue PadLeft(SealValue self, SealValue[] args)
    {
        GetPaddingArgs(args, out int width, out string pad);

        return PadLeft(self.AsString(), width, pad);
    }

    [FunctionExport("is_alpha() -> Bool")]
    public static SealValue IsAlpha(SealValue self, SealValue[] _)
    {
        string s = self.AsString();

        for (int i = 0; i < s.Length; i++)
        {
            if (!char.IsLetter(s[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static void GetPaddingArgs(SealValue[] args, out int width, out string pad)
    {
        width = args[0].AsInt32();

        pad = args.Length >= 2
            ? args[1].AsString()
            : " ";

        if (pad.Length == 0)
        {
            throw new ArgumentException("Padding string was empty.");
        }
    }

    public static string PadRight(string s, int width, string pad)
    {
        if (s.Length == width)
        {
            return s;
        }

        if (s.Length > width)
        {
            return s[..width];
        }

        var buffer = new char[width];

        for (int i = 0; i < s.Length; i++)
        {
            buffer[i] = s[i];
        }

        int j = 0;

        for (int i = s.Length; i < width; i++)
        {
            buffer[i] = pad[j++];

            if (j >= pad.Length)
            {
                j = 0;
            }
        }

        return new string(buffer);
    }
    
    public static string PadLeft(string s, int width, string pad)
    {
        if (s.Length == width)
        {
            return s;
        }

        if (s.Length > width)
        {
            return s[^width..];
        }

        var buffer = new char[width];

        int difference = width - s.Length;

        for (int i = 0; i < s.Length; i++)
        {
            buffer[difference + i] = s[i];
        }

        int j = 0;

        for (int i = 0; i < difference; i++)
        {
            buffer[i] = pad[j++];
            
            if (j >= pad.Length)
            {
                j = 0;
            }
        }

        return new string(buffer);
    }
    
    [FunctionExport("concat(args..) -> String")]
    public static SealValue Concat(SealValue[] args)
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

    [FunctionExport("format(s: String, args..) -> String")]
    public static SealValue Format(SealValue[] args) => args.Length switch
    {
        1 => args[0],
        _ => Format(args[0].AsString(), args),
    };
    
    public static string Format(string format, SealValue[] args)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < format.Length; i++)
        {
            char c = format[i];

            switch (c)
            {
            case '{':
                if (i + 1 < format.Length && format[i + 1] == '{')
                {
                    sb.Append('{');
                    i++;
                    continue;
                }
                
                int close = format.IndexOf('}', i + 1);
                
                // No end bracket is found or it is right after the open bracket
                if (close == -1 || close == i + 1)
                {
                    sb.Append('{');
                    continue;
                }

                int colon = format.IndexOf(':', i + 1, close - i - 1);

                string indexStr;
                string formatStr;
                
                if (colon == -1)
                {
                    indexStr = format[(i + 1)..close];
                    formatStr = null;
                }
                else
                {
                    indexStr = format[(i + 1)..colon];
                    formatStr = format[(colon + 1)..close];
                }

                if (!int.TryParse(indexStr, out int index)
                    || index < 0
                    || index >= args.Length - 1)
                {
                    sb.Append(format, i, 1 + close - i);
                    i = close;
                    continue;
                }

                SealValue value = args[index + 1];

                if (formatStr == null)
                {
                    sb.Append(value.ToString());
                }
                else
                {
                    object obj = value.ToObject();

                    if (obj is IFormattable formattable)
                    {
                        sb.Append(formattable.ToString(formatStr, null));
                    }
                    else
                    {
                        sb.Append(value.ToString());
                    }
                }
                
                i = close;
                break;
            case '}':
                if (i + 1 < format.Length && format[i + 1] == '}')
                {
                    i++;
                }

                sb.Append('}');
                
                break;
            default:
                sb.Append(c);
                break;
            }
        }
        
        return sb.ToString();
    }

    [FunctionExport("join(seperator: String, args..) -> String")]
    public static SealValue Join(SealValue[] args)
    {
        if (args.Length <= 1)
        {
            return string.Empty;
        }

        string seperator = args[0].AsString();
        
        var sb = new StringBuilder();

        sb.Append(args[1]);
        
        for (int i = 2; i < args.Length; i++)
        {
            sb.Append(seperator);
            sb.Append(args[i]);
        }
            
        return sb.ToString();
    }

    [FunctionExport("to_array() -> Array")]
    public static SealValue ToArray(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        
        var values = new List<SealValue>(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            values.Add(s[i]);
        }
        
        return new SealArray(values);
    }

    [FunctionExport("split(seperator: String, trim: Bool = ?) -> PackedStringArray")]
    public static SealValue Split(SealValue self, SealValue[] args)
    {
        bool trim = args.Length >= 2 && args[1].AsBool();

        StringSplitOptions options = trim
            ? StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            : StringSplitOptions.None;
        
        string[] parts = self.AsString().Split(args[0].AsString(), options);

        return new PackedStringArray(parts);
    }
}