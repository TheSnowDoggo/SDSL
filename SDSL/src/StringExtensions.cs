using System.Text;

namespace SDSL;

public static class StringExtensions
{
    public static bool TryGetEscapeCode(char escapeChar, out char escapeCode)
    {
        escapeCode = escapeChar switch
        {
            '\0' => '0',
            '\a' => 'a',
            '\b' => 'b',
            '\f' => 'f',
            '\n' => 'n',
            '\r' => 'r',
            '\t' => 't',
            '\v' => 'v',
            '"' => '"',
            _ => '_',
        };
        
        return escapeCode != '_';
    }
    
    public static bool TryGetEscapeChar(char escapeCode, out char escapeChar)
    {
        escapeChar = escapeCode switch
        {
            '0' => '\0',
            'a' => '\a',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            'v' => '\v',
            '\\' => '\\',
            '"' => '"',
            _ => '_',
        };
        
        return escapeChar != '_';
    }
    
    public static string ToEscapePreview(this string self, bool delimiters = true)
    {
        if (string.IsNullOrEmpty(self))
            return string.Empty;
        
        var sb = new StringBuilder();

        if (delimiters)
            sb.Append('\"');

        for (int i = 0; i < self.Length; i++)
        {
            char c = self[i];

            if (TryGetEscapeCode(c, out char escapeCode))
            {
                sb.Append('\\');
                sb.Append(escapeCode);
            }
            else
            {
                sb.Append(c);
            }
        }
        
        if (delimiters)
            sb.Append('\"');
        
        return sb.ToString();
    }
    
    // taken from SealScript, though i never ended up using it
    public static string ToSnakeCase(this string s)
    {
        var sb = new StringBuilder();

        int i;

        // Skip leading whitespace
        for (i = 0; i < s.Length; i++)
        {
            if (s[i] > ' ')
            {
                break;
            }
        }

        int lastCatagory = -1;
        
        for (; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '_')
            {
                sb.Append('_');
                lastCatagory = -1;
                continue;
            }

            int catagory = c switch
            {
                ' ' => 0,
                >= 'A' and <= 'Z' => 1,
                >= 'a' and <= 'z' => 2,
                >= '0' and <= '9' => 3,
                _ => -1
            };

            // Order of catagories is the order where seperation shouldn't occur
            if (catagory < lastCatagory)
            {
                sb.Append('_');
            }
            
            lastCatagory = catagory;

            switch (catagory)
            {
            case 1: // Uppercase
                sb.Append((char)(c - 'A' + 'a'));
                break;
            case 2 or 3: // Lowercase or Digit
                sb.Append(c);
                break;
            }
        }
        
        return sb.ToString();
    }
}