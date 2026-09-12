using System.Text;
using SDSL.Factory;
using SDSL.Functions;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealString
{
    public static readonly SealClass Class = SealClass.CreateGlobal("String", SealValueType.String);
    
    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealString), pAssembly, Class);
    }

    [SealConstructor]
    [FunctionInfo("value")]
    [FunctionExport("Any")]
    public static SealValue _new(SealValue[] args)
    {
        return args[0].ToString();
    }

    [FunctionExport]
    public static SealValue size(SealValue self)
    {
        return self.AsString().Length;
    }

    [FunctionInfo("index")]
    [FunctionExport("Number")]
    public static SealValue _get(SealValue self, SealValue[] args)
    {
        return self.AsString()[args[0].AsInt32()].ToString();
    }

    [FunctionExport]
    public static SealValue trim(SealValue self)
    {
        return self.AsString().Trim();
    }

    [FunctionExport]
    public static SealValue trim_start(SealValue self)
    {
        return self.AsString().TrimStart();
    }

    [FunctionExport]
    public static SealValue trim_end(SealValue self)
    {
        return  self.AsString().TrimEnd();
    }
    
    [FunctionExport]
    public static SealValue to_lower(SealValue self)
    {
        return self.AsString().ToLowerInvariant();
    }

    [FunctionExport]
    public static SealValue to_upper(SealValue self)
    {
        return self.AsString().ToUpperInvariant();
    }

    [FunctionExport]
    public static SealValue to_snake(SealValue self)
    {
        return self.AsString().ToSnakeCase();
    }

    [FunctionInfo("index")]
    [FunctionExport("Number", MinArgs = 0)]
    public static SealValue get_char_code(SealValue self, SealValue[] args)
    {
        string s = self.AsString();

        return args.Length switch
        {
            0 => s.Length >= 1 ? (double)s[0] : SealValue.Nil,
            1 => ToCharCode(s, args[0].AsInt32()),
            _ => throw new ArgumentException($"Expected 0 or 1 arguments, got {args.Length}."),
        };
        
        static SealValue ToCharCode(string s, int index)
        {
            if (index < 0 || index >= s.Length)
            {
                return SealValue.Nil;
            }

            return (double)s[index];
        }
    }
    
    [FunctionInfo("code")]
    [FunctionExport("Number")]
    public static SealValue char_code_to_string(SealValue[] args)
    {
        int code = args[0].AsInt32();

        if (code is < 0 or >= char.MaxValue)
        {
            return SealValue.Nil;
        }

        return ((char)code).ToString();
    }
    
    [FunctionInfo("value")]
    [FunctionExport("String")]
    public static SealValue has(SealValue self, SealValue[] args)
    {
        return self.AsString().Contains(args[0].AsString());
    }

    [FunctionInfo("value", "start_index", "count")]
    [FunctionExport("String", "Number", "Number", MinArgs = 1)]
    public static SealValue index_of(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        string value = args[0].AsString();
        
        return args.Length switch
        {
            1 => s.IndexOf(value, StringComparison.InvariantCulture),
            2 => _index_of2(s, value, (int)args[1].AsDouble()),
            3 => _index_of3(s, value, (int)args[1].AsDouble(), (int)args[2].AsDouble()),
            _ => throw new ArgumentException($"Expected 1, 2, or 3 arguments, got {args.Length}."),
        };
        
        static SealValue _index_of2(string s, string value, int startIndex)
        {
            if (startIndex < 0 || startIndex >= s.Length)
            {
                return -1;
            }

            return s.IndexOf(value, startIndex, StringComparison.InvariantCulture);
        }
    
        static SealValue _index_of3(string s, string value, int startIndex, int count)
        {
            if (startIndex < 0 || count < 0
                               || startIndex >= s.Length || startIndex + count > s.Length)
            {
                return -1;
            }

            return s.IndexOf(value, startIndex, count, StringComparison.InvariantCulture);
        }
    }
    
    [FunctionInfo("old_str", "new_str")]
    [FunctionExport("String", "String")]
    public static SealValue replace(SealValue self, SealValue[] args)
    {
        return self.AsString().Replace(args[0].AsString(), args[1].AsString());
    }

    [FunctionInfo("start", "count")]
    [FunctionExport("Number", "Number", MinArgs = 1)]
    public static SealValue sub_string(SealValue self, SealValue[] args)
    {
        string s = self.AsString();
        
        int start = (int)args[0].AsDouble();

        if (start >= s.Length)
        {
            return string.Empty;
        }

        return args.Length switch
        {
            1 => _sub_string1(s, start),
            2 => _sub_string2(s, start, args[1].AsInt32()),
            _ => throw new ArgumentException($"Expected 1 or 2 arguments, got {args.Length}."),
        };
        
        static SealValue _sub_string1(string s, int start)
        {
            return s[Math.Max(start, 0)..];
        }
    
        static SealValue _sub_string2(string s, int start, int count)
        {
            int end = Math.Min(start + count, s.Length);
        
            return s[Math.Max(start, 0)..end];
        }
    }
    
    [FunctionExport]
    public static SealValue is_empty(SealValue self)
    {
        return string.IsNullOrEmpty(self.AsString());
    }

    [FunctionExport]
    public static SealValue is_whitespace(SealValue self)
    {
        return string.IsNullOrWhiteSpace(self.AsString());
    }

    [FunctionInfo("width", "pad")]
    [FunctionExport("Number", "String", MinArgs = 1)]
    public static SealValue pad_right(SealValue self, SealValue[] args)
    {
        GetPaddingArgs(args, out int width, out string pad);

        string s = self.AsString();

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
    
    [FunctionInfo("width", "pad")]
    [FunctionExport("Number", "String", MinArgs = 1)]
    public static SealValue pad_left(SealValue self, SealValue[] args)
    {
        GetPaddingArgs(args, out int width, out string pad);

        string s = self.AsString();
        
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
    
    [FunctionExport]
    public static SealValue is_alpha(SealValue self)
    {
        return ForAll(self.AsString(), char.IsLetter);
    }
    
    [FunctionExport]
    public static SealValue is_alpha_numeric(SealValue self)
    {
        return ForAll(self.AsString(), char.IsLetterOrDigit);
    }

    private static bool ForAll(string s, Predicate<char> predicate)
    {
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (!predicate(c))
            {
                return false;
            }
        }

        return true;
    }
    
    [FunctionInfo("seperator", "trim")]
    [FunctionExport("String", "Bool")]
    public static SealValue split(SealValue self, SealValue[] args)
    {
        bool trim = args.Length >= 2 && args[1].AsBool();

        StringSplitOptions options = trim
            ? StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            : StringSplitOptions.None;
        
        string[] parts = self.AsString().Split(args[0].AsString(), options);

        return new PackedStringArray(parts);
    }

    [FunctionInfo("args..")]
    [FunctionExport(MaxArgs = -1)]
    public static SealValue concat(SealValue[] args)
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

    [FunctionInfo("format", "args..")]
    [FunctionExport("String", MaxArgs = -1)]
    public static SealValue format(SealValue[] args)
    {
        return args.Length switch
        {
            1 => args[0],
            _ => Format(args[0].AsString(), args),
        };
    }

    public static string Format(string format, SealValue[] args)
    {
        var sb = new StringBuilder();

        int argIndex = 0;

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
                
                // No end bracket is found
                if (close == -1)
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

                int index;

                if (string.IsNullOrWhiteSpace(indexStr))
                {
                    if (argIndex >= args.Length - 1)
                    {
                        index = args.Length - 2;
                    }
                    else
                    {
                        index = argIndex++;
                    }
                }
                else
                {
                    if (!int.TryParse(indexStr, out index)
                        || index < 0
                        || index >= args.Length - 1)
                    {
                        sb.Append(format, i, 1 + close - i);
                        i = close;
                        continue;
                    }

                    argIndex = index + 1;
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

    [FunctionInfo("seperator", "args..")]
    [FunctionExport("String", MaxArgs = -1)]
    public static SealValue join(SealValue[] args)
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
    
    [FunctionExport]
    public static SealValue to_array(SealValue self)
    {
        string s = self.AsString();
        
        var values = new List<SealValue>(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            values.Add(s[i]);
        }
        
        return new SealArray(values);
    }
    
    public static string FormatStaticInvokeFail(Function function, SealValue[] args)
    {
        var sb = new StringBuilder();

        sb.Append(function.FullName);

        sb.Append('(');
        
        for (int i = 0; i < args.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(", ");
            }

            sb.Append(args[i].ToString(true));
        }

        sb.Append(')');

        return sb.ToString();
    }
    
    public static string FormatMemberInvokeFail(Function function, SealValue self, SealValue[] args)
    {
        var sb = new StringBuilder();

        sb.Append(self.ToString(true));

        sb.Append("->");

        sb.Append(function.FullName);

        sb.Append('(');
        
        for (int i = 0; i < args.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(", ");
            }

            sb.Append(args[i].ToString(true));
        }

        sb.Append(')');

        return sb.ToString();
    }
}