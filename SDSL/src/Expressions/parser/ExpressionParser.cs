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
    
    public Expression Parse()
    {
        _operatorStack.Clear();
        _expressionStack.Clear();

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
            case TokenType.Dot:
                ParseMemberExpression(token);
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

        return _expressionStack.Pop();
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
            ParseInvokeExpression(token);
        }
        else
        {
            _operatorStack.Push(token);
            _bracketDepth++;
        }
    }

    private void ParseInvokeExpression(Token token)
    {
        FlushPrecedence(LangConfig.MaxPrecedence);
        
        PopUnary(token, out Expression functionExpression);

        if (functionExpression is MemberExpression memberExpression)
        {
            _expressionStack.Push(new MemberInvokeExpression(
                token.Location,
                memberExpression,
                GetParsedInvokeArgumentExpressions()
            ));
        }
        else
        {
            _expressionStack.Push(new StaticInvokeExpression(
                token.Location,
                functionExpression,
                GetParsedInvokeArgumentExpressions()
            ));
        }
    }

    private Expression[] GetParsedInvokeArgumentExpressions()
    {
        if (_stream.TryConsume(TokenType.CloseParen))
        {
            return [];
        }
        
        var parser = new ExpressionParser(
            _stream,
            ExpressionParsingMode.Argument,
            _functionParser
        );

        var arguments = new List<Expression>();
        
        while (!_stream.EndOfStream)
        {
            arguments.Add(parser.Parse());

            if (_stream.Peek().TokenType == TokenType.CloseParen)
                break;

            _stream.Consume(TokenType.Comma);
        }

        _stream.Consume(TokenType.CloseParen);

        return arguments.ToArray();
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

    private void AddStaticMemberReference(PrototypeClass @class, string memberName)
    {
        if (@class.Functions.TryGetValue(memberName, out PrototypeFunction function))
        {
            if (!function.IsStatic)
            {
                throw new LangException(_stream,
                    $"Cannot statically reference member function '{function}'.");
            }
            
            _expressionStack.Push(new ReferenceExpression(
                _stream.Location,
                ReferenceType.StaticFunction,
                function.AssemblyLocation
            ));

            return;
        }

        if (@class.Fields.TryGetValue(memberName, out PrototypeField field))
        {
            if (!field.IsStatic)
            {
                throw new LangException(_stream,
                    $"Cannot statically reference member field '{field}'.");
            }
            
            _expressionStack.Push(new ReferenceExpression(
                _stream.Location,
                ReferenceType.StaticField,
                field.AssemblyLocation
            ));
            
            return;
        }
        
        throw new LangException(_stream,
            $"Class {@class} does not contain member with name {memberName}.");
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

    private ReferenceExpression CreateVariableReference(string name)
    {
        return new ReferenceExpression(
            _stream.Location,
            ReferenceType.Local,
            _functionParser.GetVariableLocation(name)
        );
    }

    private void ParseLocalIdentifier(string identifier)
    {
        // Checks are in order of shadowing priority

        // Is local variable?
        if (_functionParser.TryGetVariableLocation(identifier, out int location))
        {
            _expressionStack.Push(new ReferenceExpression(
                _stream.Location,
                ReferenceType.Local,
                location
            ));
            
            return;
        }

        bool isStatic = _functionParser.PrototypeFunction.IsStatic;
        
        // Is function in containing class?
        if (_containingClass.Functions.TryGetValue(identifier,
                out PrototypeFunction function))
        {
            if (function.IsStatic)
            {
                // Implicit Class.static_function
                _expressionStack.Push(new ReferenceExpression(
                    _stream.Location,
                    ReferenceType.StaticFunction,
                    function.AssemblyLocation
                ));
            }
            else
            {
                if (isStatic)
                {
                    throw new LangException(_stream,
                        $"Cannot reference instance function '{identifier}' in a static context.");
                }
                
                // Implicit self.instance_function
                _expressionStack.Push(new MemberExpression(
                    _stream.Location,
                    CreateVariableReference(Function.SelfName),
                    identifier
                ));
            }
            
            return;
        }

        // Is member in containing class?
        if (_containingClass.Fields.TryGetValue(identifier,
                out PrototypeField field))
        {
            if (field.IsStatic)
            {
                // Implicit Class.static_field
                _expressionStack.Push(new ReferenceExpression(
                    _stream.Location,
                    ReferenceType.StaticFunction,
                    field.AssemblyLocation
                ));
            }
            else
            {
                if (isStatic)
                {
                    throw new LangException(_stream,
                        $"Cannot reference instance function '{identifier}' in a static context.");
                }
                
                // Implicit self.instance_field
                _expressionStack.Push(new MemberExpression(
                    _stream.Location,
                    CreateVariableReference(Function.SelfName),
                    identifier
                ));
            }
            
            return;
        }

        throw new LangException(_stream,
            $"No local variable or member with name '{identifier}' found.");
    }

    private void ParseLiteral(Token token)
    {
        var expression = new LiteralExpression(
            _stream.Location,
            token.Value
        );
        
        _expressionStack.Push(expression);
    }

    private void ParseMemberExpression(Token token)
    {
        FlushPrecedence(LangConfig.MaxPrecedence);
        
        PopUnary(token, out Expression instanceExpression);

        string identifier = _stream.ConsumeIdentifer();
        
        _expressionStack.Push(new MemberExpression(
            token.Location,
            instanceExpression,
            identifier
        ));
    }
    
    private void TransferOperator()
    {
        Token token = _operatorStack.Pop();

        switch (token.TokenType)
        {
        default:
            throw new LangException(token,
                $"Cannot create expression for operator {token.TokenType}.");
        }
    }
    
    private void PopUnary(
        Token token,
        out Expression operand)
    {
        if (_expressionStack.Count < 1)
        {
            throw new LangException(token,
                $"{token.TokenType} expected 1 operand, got {_expressionStack.Count}.");
        }
        
        operand = _expressionStack.Pop();
    }
    
    private void PopBinary(
        Token token,
        out Expression left,
        out Expression right)
    {
        if (_expressionStack.Count < 2)
        {
            throw new LangException(token,
                $"{token.TokenType} expected 2 operands, got {_expressionStack.Count}.");
        }
        
        right = _expressionStack.Pop();
        left = _expressionStack.Pop();
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
               && other.TokenType != TokenType.OpenParen
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