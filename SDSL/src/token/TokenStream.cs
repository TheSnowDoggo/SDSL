namespace SDSL;

public class TokenStream : ISourceLocated
{
    private readonly ArraySegment<Token> _tokens;
    private int _position;

    public TokenStream(ArraySegment<Token> tokens, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);

        if (position > tokens.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position,
                "Position exceeds token source length.");
        }
        
        _tokens = tokens;
        _position = position;
    }

    public TokenStream(ArraySegment<Token> tokens)
    {
        _tokens = tokens;
    }
    
    public ArraySegment<Token> Tokens => _tokens;
    
    public int Position => _position;
    public int Length => _tokens.Count;
    
    public bool EndOfStream => _position >= _tokens.Count;

    public SourceLocation Location => GetLastToken()?.Location ?? SourceLocation.Empty;
    
    public Token this[int position] => _tokens[position];

    public Token Peek()
    {
        ThrowIfEndOfStream();
        return _tokens[_position];
    }

    public bool TryPeek(out Token token)
    {
        if (_position >= _tokens.Count)
        {
            token = null;
            return false;
        }
        
        token = _tokens[_position];
        return true;
    }

    public Token Read()
    {
        ThrowIfEndOfStream();
        return _tokens[_position++];
    }
    
    public bool TryRead(out Token token)
    {
        if (_position >= _tokens.Count)
        {
            token = null;
            return false;
        }
        
        token = _tokens[_position++];
        return true;
    }

    public void Advance()
    {
        ThrowIfEndOfStream();
        _position++;
    }
    
    public Token Consume(TokenType expectedType)
    {
        ThrowIfEndOfStream();
        
        Token token = _tokens[_position];

        if (token.TokenType != expectedType)
        {
            throw new LangException(token, $"Expected token of type {expectedType}, got {token.TokenType}.");
        }
        
        _position++;

        return token;
    }

    public string ConsumeIdentifer()
    {
        return Consume(TokenType.Identifier).Value.AsString();
    }
    
    public bool TryConsume(TokenType expectedType, out Token token)
    {
        if (_position >= _tokens.Count)
        {
            token = null;
            return false;
        }
        
        token = _tokens[_position];

        if (token.TokenType != expectedType)
        {
            return false;
        }
        
        _position++;
        
        return true;
    }

    public bool TryConsume(TokenType expectedType)
    {
        return TryConsume(expectedType, out _);
    }
    
    private Token GetLastToken()
    {
        if (_tokens.Count == 0)
        {
            return null;
        }

        if (_position == 0)
        {
            return _tokens[0];
        }

        return _tokens[_position - 1];
    }

    private void ThrowIfEndOfStream()
    {
        if (_position >= _tokens.Count)
        {
            throw new LangException(GetLastToken(), "Unexpected end of stream.");
        }
    }
}