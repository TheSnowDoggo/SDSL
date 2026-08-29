using System.Text;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealString
{
    [CustomClassExport]
    public static readonly SealClass Class = new SealClass(
        LangConfig.GlobalNamespace,
        "String",
        ValueType.String,
        true
    );
    
    [FunctionExport("new(x: Any) -> String")]
    public static SealValue New(ReadOnlySpan<SealValue> args)
        => args[0].ToString();
    
    [FunctionExport("trim() -> String")]
    public static SealValue Trim(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().Trim();
    
    [FunctionExport("trim_start() -> String")]
    public static SealValue TrimStart(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().TrimStart();
    
    [FunctionExport("trim_end() -> String")]
    public static SealValue TrimEnd(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().TrimEnd();
    
    [FunctionExport("to_lower() -> String")]
    public static SealValue ToLower(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().ToLowerInvariant();
    
    [FunctionExport("to_upper() -> String")]
    public static SealValue ToUpper(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().ToUpperInvariant();
    
    [FunctionExport("to_snakecase() -> String")]
    public static SealValue ToSnake(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().ToSnakeCase();
    
    [FunctionExport("to_char_code()")]
    public static SealValue ToCharCode(SealValue self, ReadOnlySpan<SealValue> args)
    {
        string s = self.AsString();
        return s.Length == 1 ? (double)s[0] : SealValue.Nil;
    }

    [FunctionExport("has(s: String) -> Bool")]
    public static SealValue Has(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().Contains(args[0].AsString());
    
    [FunctionExport("index_of(s: String) -> Number")]
    public static SealValue IndexOf(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().IndexOf(args[0].AsString(), StringComparison.Ordinal);
    
    [FunctionExport("replace(old_str: String, new_str: String) -> String")]
    public static SealValue Replace(SealValue self, ReadOnlySpan<SealValue> args)
        => self.AsString().Replace(args[0].AsString(), args[1].AsString());

    [FunctionExport("substr(start: Number, count: Number) -> String")]
    public static SealValue Substring(SealValue self, ReadOnlySpan<SealValue> args)
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
    public static SealValue IsEmpty(SealValue self, ReadOnlySpan<SealValue> args)
        => string.IsNullOrEmpty(self.AsString());

    [FunctionExport("is_whitespace() -> Bool")]
    public static SealValue IsWhiteSpace(SealValue self, ReadOnlySpan<SealValue> args)
        => string.IsNullOrWhiteSpace(self.AsString());
    
    [FunctionExport("concat(args..) -> String")]
    public static SealValue Concat(ReadOnlySpan<SealValue> args)
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
    public static SealValue Format(ReadOnlySpan<SealValue> args)
    {
        switch (args.Length)
        {
        case 1:
            return args[0];
        default:
            string s = args[0].AsString();
            
            var sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                switch (c)
                {
                case '{':
                    i++;
                    
                    if (i >= s.Length
                        || s[i] == '{')
                    {
                        sb.Append('{');
                        continue;
                    }
                    
                    int close = s.IndexOf('}', i);

                    if (close == -1)
                    {
                        sb.Append('{');
                        continue;
                    }

                    string indexStr = s[i..close];
                    
                    i = close;

                    if (!int.TryParse(indexStr, out int index)
                        || index < 0
                        || index > args.Length - 1)
                    {
                        sb.Append('{');
                        sb.Append(indexStr);
                        sb.Append('}');
                        continue;
                    }
                    
                    sb.Append(args[index + 1]);
                    break;
                case '}':
                    // Remove double braces
                    if (i + 1 < s.Length
                        && s[i + 1] == '}')
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
    }

    [FunctionExport("join(seperator: String, args..) -> String")]
    public static SealValue Join(ReadOnlySpan<SealValue> args)
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
}