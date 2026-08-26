using SDSL.Prototypes;

namespace SDSL.Expressions;

public class ExpressionParser
{
    private readonly TokenStream _stream;
    private readonly ExpressionParsingMode _parsingMode;
    private readonly FunctionParser _functionParser;

    private readonly PrototypeClass _containingClass;

    private readonly Stack<Token> _operatorStack = [];
    private readonly Stack<Expression> _expressionStack = [];

    private int _bracketDepth;
    
    public ExpressionParser(
        TokenStream stream,
        ExpressionParsingMode parsingMode,
        FunctionParser functionParser)
    {
        _stream = stream;
        _parsingMode = parsingMode;
        _functionParser = functionParser;

        _containingClass = _functionParser.PrototypeFunction.Class;
    }

    public PackedExpression Parse()
    {
        _operatorStack.Clear();
        _expressionStack.Clear();

        SourceLocation location = _stream.Location;

        while (_stream.TryPeek(out Token token))
        {
            if (ShouldExit(token))
            {
                break;
            }
            
            _stream.Advance();
            
            switch (token.TokenType)
            {
            case TokenType.OpenParen:
                ParseOpenParen(token);
                break;
            case TokenType.CloseParen:
                ParseCloseParen(token);
                break;
            case TokenType.Identifier:
                ParseIdentifer(token);
                break;
            case TokenType.Literal:
                ParseLiteral(token);
                break;
            default:
                PushOperator(token);
                break;
            }
        }

        FlushAll();

        if (_expressionStack.Count != 1)
        {
            throw new LangException(_stream,
                "Failed to parse expression.");
        }

        Expression expression = _expressionStack.Pop();

        return new PackedExpression(
            expression,
            location,
            _containingClass.Assembly.Assembly
        );
    }
    
    private static bool IsCallable(Token token)
    {
        return token.TokenType is TokenType.Identifier
            or TokenType.CloseParen
            or TokenType.CloseBrace;
    }
    
    private static bool IsOperand(Token token)
    {
        return token.TokenType is TokenType.CloseParen
            or TokenType.CloseBrace
            or TokenType.Identifier
            or TokenType.Literal;
    }

    private static bool ShouldFlush(Token token, int precedence, Token other)
    {
        if (other.TokenType == TokenType.OpenParen)
        {
            return false;
        }
        
        int otherPrecedence = LangConfig.PrecedenceMap[other.TokenType];

        if (precedence < otherPrecedence)
        {
            return true;
        }

        if (precedence > otherPrecedence)
        {
            return false;
        }

        // If both operators are right associative and the precedence is the same, do not flush.
        if (LangConfig.RightAssociativeSet.Contains(token.TokenType)
            && LangConfig.RightAssociativeSet.Contains(other.TokenType))
        {
            return false;
        }

        return true;
    }
    
    private bool ShouldExit(Token token)
    {
        return _parsingMode switch
        {
            ExpressionParsingMode.Statement 
                => token.TokenType is TokenType.Semicolon,
            ExpressionParsingMode.Argument 
                => token.TokenType is TokenType.Comma 
                    or TokenType.CloseParen,
            _ => false
        };
    }
    
    private void ParseOpenParen(Token token)
    {
        // Check for function call
        if (_stream.Position > 1 && IsCallable(_stream[_stream.Position - 2]))
        {
            //ParseInvokeExpression(token);
        }
        else
        {
            _operatorStack.Push(token);
            _bracketDepth++;
        }
    }

    private void ParseCloseParen(Token token)
    {
        if (_bracketDepth == 0)
        {
            throw new LangException(token, "No matching open parenthesis found for close parenthesis.");
        }

        while (_operatorStack.TryPeek(out Token peek)
               && peek.TokenType != TokenType.OpenParen)
        {
            TransferOperator();
        }

        // We should be able to safely pop from the bracket depth check
        _operatorStack.Pop();

        _bracketDepth--;
    }
    
    private void ParseIdentifer(Token token)
    {
        string identifer = token.Value.AsString();

        // Local ref OR Implicit Static or Instance Member ref
        if (!_stream.TryPeek(out Token next))
        {
            ParseLocalIdentifier(identifer);
            return;
        }

        switch (next.TokenType)
        {
        // Static Function/Field ref 
        case TokenType.Scope:
            ParseFullStaticClassMember(identifer);
            break;
        // Implicit Static Function/Field ref OR Instance member ref
        case TokenType.Dot:
            ParseImplicitStaticClassMember(identifer);
            break;
        // Local Variable
        default:
            ParseLocalIdentifier(identifer);
            break;
        }
    }
    
    private bool TryCreateStaticMemberReference(PrototypeClass @class, string memberName, out ReferenceExpression expression)
    {
        if (@class.Functions.TryGetValue(memberName, out PrototypeFunction function))
        {
            if (!function.IsStatic)
            {
                throw new LangException(_stream,
                    $"Cannot statically reference member function {function}.");
            }
            
            expression = new ReferenceExpression(
                ReferenceType.StaticFunction,
                function.AssemblyLocation
            );

            return true;
        }

        if (@class.Fields.TryGetValue(memberName, out PrototypeField field))
        {
            if (!field.IsStatic)
            {
                throw new LangException(_stream,
                    $"Cannot statically reference member field {field}.");
            }
            
            expression =  new ReferenceExpression(
                ReferenceType.StaticField,
                field.AssemblyLocation
            );
            
            return true;
        }

        expression = null;
        return false;
    }

    private void AddStaticMemberReference(PrototypeClass @class, string memberName)
    {
        if (TryCreateStaticMemberReference(@class, memberName, out ReferenceExpression expression))
        {
            _expressionStack.Push(expression);
        }
        else
        {
            throw new LangException(_stream,
                $"Class {@class} does not contain member with name {memberName}.");
        }
    }
    
    private void ParseFullStaticClassMember(string namespaceName)
    {
        // Consume scope
        _stream.Read();
        
        string className = _stream.ConsumeIdentifer();

        PrototypeClass @class = _containingClass.ResolveFullClass(_stream.Location, namespaceName, className);

        // A class on its own is not a valid expression, so it always involves a member reference
        // e.g. Namespace::Class.function
        _stream.Consume(TokenType.Dot);

        string memberName = _stream.ConsumeIdentifer();

        AddStaticMemberReference(@class, memberName);
    }

    private void ParseImplicitStaticClassMember(string className)
    {
        if (!_containingClass.TryResolveImplicitClass(_stream.Location, className, out PrototypeClass @class))
        {
            ParseLocalIdentifier(className);
            return;
        }
        
        // Consume dot
        _stream.Read();
        
        string memberName = _stream.ConsumeIdentifer();
        
        AddStaticMemberReference(@class, memberName);
    }

    private bool TryCreateInstanceMemberReference(string memberName, out ReferenceExpression expression)
    {
        throw new NotImplementedException();
    }

    private void ParseLocalIdentifier(string identifier)
    {
        // Checks are in order of shadowing priority

        if (_functionParser.TryGetVariableLocation(identifier, out int location))
        {
            var expression = new ReferenceExpression(
                ReferenceType.LocalVariable,
                location
            );
            
            _expressionStack.Push(expression);
            return;
        }

        if (!_functionParser.PrototypeFunction.IsStatic)
        {
            
        }

        if (TryCreateStaticMemberReference(_containingClass, identifier, out ReferenceExpression staticMemberReference))
        {
            _expressionStack.Push(staticMemberReference);
            return;
        }

        throw new LangException(_stream,
            $"No local variable or member with name {identifier} found.");
    }

    private void ParseLiteral(Token token)
    {
        var expression = new LiteralExpression(token.Value);
        
        _expressionStack.Push(expression);
    }
    
    private void TransferOperator()
    {
        Token token = _operatorStack.Pop();

        throw new NotImplementedException();
    }
    
    private void PushOperator(Token token)
    {
        // Try convert to an associated unary operator (such as for subtract/minus)
        if (LangConfig.UnaryMap.TryGetValue(token.TokenType, out TokenType unaryType)
            && (_stream.Position == 0 || !IsOperand(_stream[_stream.Position - 2])))
        {
            token.TokenType = unaryType;
        }
        
        // Most likely occurs from reading past the expression when a semicolon is missed
        if (!LangConfig.PrecedenceMap.TryGetValue(token.TokenType, out int precedence))
        {
            throw new LangException(_stream, 
                $"Expected operator, got {token.TokenType}. Did you miss a semicolon?");
        }
        
        while (_operatorStack.TryPeek(out Token other) 
               && ShouldFlush(token, precedence, other))
        {
            TransferOperator();
        }
        
        _operatorStack.Push(token);
    }
    
    private void FlushPrecedence(int precedence)
    {
        while (_operatorStack.TryPeek(out Token other) 
               && LangConfig.PrecedenceMap[other.TokenType] >= precedence)
        {
            TransferOperator();
        }
    }
    
    private void FlushAll()
    {
        while (_operatorStack.Count != 0)
        {
            TransferOperator();
        }
    }
}