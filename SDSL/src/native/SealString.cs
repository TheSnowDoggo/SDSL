using System.Text;
using SDSL.Prototypes;

namespace SDSL;

public static class SealString
{
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
}