using System.Text;

namespace SDSL;

public class Tokenizer : IDisposable
{
    private readonly TextReader _reader;
    
    private List<Token> _tokens;

    private int _line;
    private int _column;
    
    public Tokenizer(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }
    
    public Tokenizer(Stream stream)
    {
        _reader = new StreamReader(stream);
    }
    
    public Tokenizer(string s)
    {
        _reader = new StringReader(s);
    }
    
    public Token[] Tokenize()
    {
        _tokens = [];

        _line = 1;
        _column = 0;

        while (TryPeek(out char initial))
        {
            if (initial <= ' ')
            {
                Advance();
                continue;
            }
            
            var location = new SourceLocation(_line, _column);
            
            Advance();

            switch (initial)
            {
            case '(':
                CreateToken(location, TokenType.OpenParen);
                break;
            case ')':
                CreateToken(location, TokenType.CloseParen);
                break;
            case '{':
                CreateToken(location, TokenType.OpenBrace);
                break;
            case '}':
                CreateToken(location, TokenType.CloseBrace);
                break;
            case '[':
                CreateToken(location, TokenType.OpenSquare);
                break;
            case ']':
                CreateToken(location, TokenType.CloseSquare);
                break;
            case ':':
                CreateToken(location, TryConsume(':')
                    ? TokenType.Scope
                    : TokenType.Colon);
                break;
            case ';':
                CreateToken(location, TokenType.Semicolon);
                break;
            case ',':
                CreateToken(location, TokenType.Comma);
                break;
            case '.':
                CreateToken(location, TokenType.Dot);
                break;
            case '*':
                CreateToken(location, TryConsume('*')
                    ? TokenType.MultiplyAssign
                    : TokenType.Multiply);
                break;
            case '#':
                if (TryPeek(out char next)
                    && next == '*')
                {
                    Advance();
                    SkipMultiComment(location);
                }
                else
                {
                    SkipSingleComment();
                }
                break;
            case '/':
                CreateToken(location, TryConsume('/')
                    ? TryConsume('=')
                        ? TokenType.IDivideAssign
                        : TokenType.IDivide
                    : TryConsume('=')
                        ? TokenType.DivideAssign
                        : TokenType.Divide);
                break;
            case '%':
                CreateToken(location, TryConsume('=')
                    ? TokenType.ModuloAssign
                    : TokenType.Modulo);
                break;
            case '+':
                CreateToken(location, TryConsume('=')
                    ? TokenType.AddAssign
                    : TokenType.Add);
                break;
            case '-':
                CreateToken(location, TryConsume('>')
                    ? TokenType.Arrow
                    : TryConsume('=')
                        ? TokenType.SubtractAssign
                        : TokenType.Subtract);
                break;
            case '<':
                CreateToken(location, TryConsume('=')
                    ? TokenType.LessThanOrEqual
                    : TokenType.LessThan);
                break;
            case '>':
                CreateToken(location, TryConsume('=')
                    ? TokenType.GreaterThanOrEqual
                    : TokenType.GreaterThan);
                break;
            case '=':
                CreateToken(location, TryConsume('=')
                    ? TokenType.Equal
                    : TokenType.Assign);
                break;
            case '!':
                CreateToken(location, TryConsume('=')
                    ? TokenType.NotEqual
                    : TokenType.Not);
                break;
            case '&':
                CreateToken(location, TryConsume('&')
                    ? TokenType.ConditionalAnd
                    : TryConsume('=')
                        ? TokenType.AndAssign
                        : TokenType.And);
                break;
            case '^':
                CreateToken(location, TryConsume('=')
                    ? TokenType.XorAssign
                    : TokenType.Xor);
                break;
            case '|':
                CreateToken(location, TryConsume('|')
                    ? TokenType.ConditionalOr
                    : TryConsume('=')
                        ? TokenType.OrAssign
                        : TokenType.Or);
                break;
            case >= 'A' and <= 'Z'
                or >= 'a' and <= 'z'
                or '_':
                CreateAlphaNumericToken(location, initial);
                break;
            case '"' or '\'':
                CreateStringToken(location, initial);
                break;
            case >= '0' and <= '9':
                CreateNumberToken(location, initial);
                break;
            default:
                throw new LangException(location,
                    $"Read unrecognised symbol: '{initial}'");
            }
        }

        Token[] tokens = _tokens.ToArray();
        _tokens = null;
        
        return tokens;
    }

    public void Dispose()
    {
        _reader.Dispose();
        GC.SuppressFinalize(this);
    }

    private static bool IsAlphaNumeric(char c)
    {
        return c is >= 'A' and <= 'Z'
            or >= 'a' and <= 'z'
            or >= '0' and <= '9'
            or '_';
    }

    private static bool IsDigitOrDecimal(char c)
    {
        return c is >= '0' and <= '9'
            or '.';
    }

    private void CreateToken(SourceLocation location, TokenType tokenType, SealValue value = default)
    {
        _tokens.Add(new Token(location, tokenType, value));
    }
    
    private void CreateAlphaNumericToken(SourceLocation location, char initial)
    {
        var sb = new StringBuilder();
        sb.Append(initial);

        while (TryPeek(out char peek)
               && IsAlphaNumeric(peek))
        {
            Advance();
            sb.Append(peek);
        }

        string str = sb.ToString();

        if (LangConfig.KeywordMap.TryGetValue(str, out TokenType keywordType))
        {
            CreateToken(location, keywordType);
            return;
        }

        if (LangConfig.LiteralMap.TryGetValue(str, out SealValue literal))
        {
            CreateToken(location, TokenType.Literal, literal);
            return;
        }
        
        CreateToken(location, TokenType.Identifier, str);
    }

    private void CreateStringToken(SourceLocation location, char delimiter)
    {
        var sb = new StringBuilder();

        while (TryPeek(out char peek)
               && peek != delimiter)
        {
            Advance();

            if (peek == '\\'
                && TryPeek(out char next)
                && StringExtensions.TryGetEscapeCode(next, out char escapeChar))
            {
                Advance();
                sb.Append(escapeChar);
            }
            else
            {
                sb.Append(peek);
            }
        }

        if (!Advance())
        {
            throw new LangException(location,
                $"String literal missing end delimiter: {delimiter}.");
        }
        
        string str = sb.ToString();

        CreateToken(location, TokenType.Literal, str);
    }

    private void CreateNumberToken(SourceLocation location, char initial)
    {
        var sb = new StringBuilder();
        sb.Append(initial);

        while (TryPeek(out char peek)
               && IsDigitOrDecimal(peek))
        {
            Advance();
            sb.Append(peek);
        }
        
        string str = sb.ToString();

        if (!double.TryParse(str, out double value))
        {
            throw new LangException(location,
                $"Failed to parse number '{str}'.");
        }
        
        CreateToken(location, TokenType.Literal, value);
    }

    private void SkipSingleComment()
    {
        while (TryPeek(out char peek)
               && peek != '\n')
        {
            Advance();
        }
    }
    
    private void SkipMultiComment(SourceLocation location)
    {
        char last = '\0';
        
        while (TryPeek(out char peek)
               && !(last == '*' && peek == '#'))
        {
            Advance();
            last = peek;
        }
        
        // Remove trailing '/'
        if (!Advance())
        {
            throw new LangException(location,
                "Multiline comment missing ending delimiter '*/'");
        }
    }
    
    private bool TryPeek(out char c)
    {
        int value = _reader.Peek();

        if (value < 0)
        {
            c = '\0';
            return false;
        }

        c = (char)value;
        return true;
    }

    private bool Advance()
    {
        int value = _reader.Read();

        if (value < 0)
        {
            return false;
        }
        
        char c = (char)value;

        switch (c)
        {
        case '\n':
            _column = 0;
            _line++;
            break;
        case '\r':
            _column = 0;
            break;
        default:
            _column++;
            break;
        }

        return true;
    }

    private bool TryConsume(char expected)
    {
        if (!TryPeek(out char c)
            || c != expected)
        {
            return false;
        }

        Advance();

        return true;
    }
}