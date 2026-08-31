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
        true
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
    
    [FunctionExport("index_of(s: String) -> Number")]
    public static SealValue IndexOf(SealValue self, SealValue[] args)
        => self.AsString().IndexOf(args[0].AsString(), StringComparison.Ordinal);
    
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

    [FunctionExport("join(seperator: String, args..) -> String")]
    public static SealValue Join(SealValue[] args)
    {
        switch (args.Length)
        {
        case 1:
            return string.Empty;
        case 2:
            return args[1].ToString();
        default:
            string seperator = args[0].AsString();
            
            var sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                if (i != 0)
                    sb.Append(seperator);
                sb.Append(args[i]);
            }
            
            return sb.ToString();
        }
    }

    public static string Format(string s, SealValue[] args)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            
            if (c == '{' && (i == 0 || s[i - 1] != '/'))
            {
                int close = s.IndexOf('}', i + 1);
                
                // No end bracket is found or it is right after the open bracket
                if (close == -1 || close == i + 1)
                {
                    sb.Append('{');
                    continue;
                }

                int colon = s.IndexOf(':', i + 1, close - i - 1);

                string indexStr;
                string formatStr;
                
                if (colon == -1)
                {
                    indexStr = s[(i + 1)..close];
                    formatStr = null;
                }
                else
                {
                    indexStr = s[(i + 1)..colon];
                    formatStr = s[(colon + 1)..close];
                }

                if (!int.TryParse(indexStr, out int index)
                    || index < 0
                    || index >= args.Length - 1)
                {
                    sb.Append(s, i, 1 + close - i);
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
            }
            else
            {
                sb.Append(c);
            }
        }
        
        return sb.ToString();
    }
}