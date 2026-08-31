namespace SDSL;

public class TokenStream : ISourceLocated
{
    private readonly ArraySegment<Token> _tokens;
    private int _position;

    public TokenStream(ArraySegment<Token> tokens, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, tokens.Count);
        
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

    public SourceLocation Location => GetLastToken()?.Location ?? SourceLocation.Invalid;
    
    public Token this[int position] => _tokens[position];

    public Token Peek()
    {
        if (_position >= _tokens.Count)
        {
            throw new ParserException(GetLastToken(), "Unexpected end of stream peeking token.");
        }
        
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
        if (_position >= _tokens.Count)
        {
            throw new ParserException(GetLastToken(), "Unexpected end of stream reading token.");
        }
        
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
        if (_position >= _tokens.Count)
        {
            throw new ParserException(GetLastToken(), "Unexpected end of stream advancing stream.");
        }
        
        _position++;
    }
    
    public Token Consume(TokenType expectedType)
    {
        if (_position >= _tokens.Count)
        {
            throw new ParserException(GetLastToken(), $"Expected token of type {expectedType}, got end of stream.");
        }
        
        Token token = _tokens[_position];

        if (token.TokenType != expectedType)
        {
            throw new ParserException(token, $"Expected token of type {expectedType}, got {token.TokenType}.");
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

    public void SkipArgument()
    {
        var bracketStack = new Stack<TokenType>();

        while (TryPeek(out Token token))
        {
            if (bracketStack.Count == 0
                && token.TokenType is TokenType.Comma or TokenType.CloseParen)
            {
                break;
            }
            
            Advance();

            switch (token.TokenType)
            {
            case TokenType.OpenParen:
            case TokenType.OpenSquare:    
                bracketStack.Push(token.TokenType);
                break;
            case TokenType.CloseParen:
            case TokenType.CloseSquare:
                if (!bracketStack.TryPop(out TokenType lastBracket)
                    || token.TokenType != lastBracket)
                {
                    return;
                }
                break;
            }
        }
    }

    public void SkipStatement(bool noTerminators = false)
    {
        if (!TryPeek(out Token start))
            return;

        int startLine = start.Location.Line;
        
        while (TryPeek(out Token token)
               && token.TokenType != TokenType.Semicolon
               && !(noTerminators && token.Location.Line != startLine))
        {
            Advance();
        }
    }

    public void SkipBlock()
    {
        int bracketDepth = 0;

        while (TryPeek(out Token token))
        {
            switch (token.TokenType)
            {
            case TokenType.OpenBrace:
                bracketDepth++;
                break;
            case TokenType.CloseBrace:
                if (bracketDepth == 0)
                    return;
                bracketDepth--;
                break;
            }
            
            Advance();
        }
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
}