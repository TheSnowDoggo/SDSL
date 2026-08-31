using System.Text;
using SDSL.Prototypes;

namespace SDSL.Classes;

[SealClass]
public static class SealString
{
    [ClassExport]
    public static readonly SealClass Class = new SealClass(
        GlobalConfig.GlobalNamespace,
        "String",
        ValueType.String,
        false
    );
    
    [FunctionExport("new(x: Any) -> String")]
    public static SealValue New(SealValue[] args)
        => args[0].ToString();
    
    [FunctionExport("trim() -> String")]
    public static SealValue Trim(SealValue self, SealValue[] args)
        => self.AsString().Trim();
    
    [FunctionExport("trim_start() -> String")]
    public static SealValue TrimStart(SealValue self, SealValue[] args)
        => self.AsString().TrimStart();
    
    [FunctionExport("trim_end() -> String")]
    public static SealValue TrimEnd(SealValue self, SealValue[] args)
        => self.AsString().TrimEnd();
    
    [FunctionExport("to_lower() -> String")]
    public static SealValue ToLower(SealValue self, SealValue[] args)
        => self.AsString().ToLowerInvariant();
    
    [FunctionExport("to_upper() -> String")]
    public static SealValue ToUpper(SealValue self, SealValue[] args)
        => self.AsString().ToUpperInvariant();
    
    [FunctionExport("to_snakecase() -> String")]
    public static SealValue ToSnake(SealValue self, SealValue[] args)
        => self.AsString().ToSnakeCase();
    
    [FunctionExport("to_char_code()")]
    public static SealValue ToCharCode(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        return s.Length == 1 ? (double)s[0] : SealValue.Nil;
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

    [FunctionExport("substr(start: Number, count: Number) -> String")]
    public static SealValue Substring(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        
        int start = (int)args[0].AsNumber();
        if (start >= s.Length)
            return string.Empty;
        
        int count = (int)args[1].AsNumber();
        if (count < 0)
            return string.Empty;
        
        int end = Math.Min(start + count, s.Length);
        
        start = Math.Max(start, 0);
        
        return s[start..end];
    }
    
    [FunctionExport("is_empty() -> Bool")]
    public static SealValue IsEmpty(SealValue self, SealValue[] args)
        => string.IsNullOrEmpty(self.AsString());

    [FunctionExport("is_whitespace() -> Bool")]
    public static SealValue IsWhiteSpace(SealValue self, SealValue[] args)
        => string.IsNullOrWhiteSpace(self.AsString());
    
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
}