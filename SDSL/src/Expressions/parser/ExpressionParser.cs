using SDSL.Prototypes;

namespace SDSL.Expressions;

public class ExpressionParser
{
    private readonly TokenStream _stream;
    private readonly ExpressionParsingMode _parsingMode;
    private readonly PrototypeClass _containingClass;
    
    private readonly UserFunctionParser _functionParser;

    private readonly Stack<Token> _operatorStack = [];
    private readonly Stack<Expression> _expressionStack = [];

    private int _bracketDepth;
    private int _startLine;
    
    public ExpressionParser(
        TokenStream stream,
        ExpressionParsingMode parsingMode,
        UserFunctionParser functionParser)
    {
        _stream = stream;
        _parsingMode = parsingMode;
        _functionParser = functionParser;

        _containingClass = _functionParser.PrototypeFunction.Class;
    }
    
    public ExpressionParser(
        TokenStream stream,
        ExpressionParsingMode parsingMode,
        PrototypeClass containingClass)
    {
        _stream = stream;
        _parsingMode = parsingMode;
        _containingClass = containingClass;
    }
    
    public Expression Parse(bool allowEmpty = false)
    {
        _operatorStack.Clear();
        _expressionStack.Clear();

        _bracketDepth = 0;
        _startLine = -1;

        while (_stream.TryPeek(out Token token))
        {
            if (_startLine == -1)
                _startLine = token.Location.Line;
            
            if (ShouldExit(token))
                break;
            
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
            case TokenType.New:
                ParseConstructor(token);
                break;
            case TokenType.OpenSquare:
                ParseOpenSquare(token);
                break;
            case TokenType.OpenBrace:
                ParseMapExpression(token);
                break;
            default:
                PushOperator(token);
                break;
            }
        }

        FlushAll();

        switch (_expressionStack.Count)
        {
        case 0:
            if (allowEmpty)
                return LiteralExpression.Nil;
            throw new LangException(_stream,
                "Expression was empty.");
        case 1:
            return _expressionStack.Pop();
        default:
            throw new LangException(_stream,
                "Failed to parse expression.");
        }
    }
    
    private static bool IsCallable(Token token)
    {
        return token.TokenType is TokenType.Identifier
            or TokenType.CloseParen
            or TokenType.CloseBrace
            or TokenType.CloseSquare;
    }
    
    private static bool IsOperand(Token token)
    {
        return token.TokenType is TokenType.CloseParen
            or TokenType.CloseBrace
            or TokenType.CloseSquare
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
                => token.TokenType is TokenType.Semicolon
                || (_containingClass.NoTerminators && token.Location.Line != _startLine),
            ExpressionParsingMode.Argument 
                => token.TokenType is TokenType.Comma
                       or TokenType.CloseSquare
                       or TokenType.CloseBrace
                       or TokenType.Colon
                    || (_bracketDepth == 0 && token.TokenType is TokenType.CloseParen),
            ExpressionParsingMode.Condition
                => token.TokenType is TokenType.OpenBrace,
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
                GetParsedArgumentList(TokenType.CloseParen),
                memberExpression
            ));
        }
        else
        {
            _expressionStack.Push(new StaticInvokeExpression(
                token.Location,
                GetParsedArgumentList(TokenType.CloseParen),
                functionExpression
            ));
        }
    }

    private ExpressionParser CreateSubParser(ExpressionParsingMode parsingMode)
    {
        return _functionParser == null
            ? new ExpressionParser(_stream, parsingMode, _containingClass)
            : new ExpressionParser(_stream, parsingMode, _functionParser);
    }

    private Expression[] GetParsedArgumentList(TokenType closeType)
    {
        if (_stream.TryConsume(closeType))
            return [];

        ExpressionParser parser = CreateSubParser(ExpressionParsingMode.Argument);

        var arguments = new List<Expression>();
        
        while (!_stream.EndOfStream)
        {
            arguments.Add(parser.Parse());

            if (_stream.Peek().TokenType == closeType)
                break;

            _stream.Consume(TokenType.Comma);
        }

        _stream.Consume(closeType);

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
        // Implicit Static Function/Field ref OR Local Variable
        case TokenType.Dot:
            ParseImplicitStaticClassMember(identifer);
            break;
        // Local Variable
        default:
            ParseLocalIdentifier(identifer);
            break;
        }
    }
    
    private void AddStaticMemberReference(PrototypeClass pClass, string memberName)
    {
        if (pClass.Functions.TryGetValue(memberName, out PrototypeFunction function))
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

        if (pClass.Fields.TryGetValue(memberName, out PrototypeField field))
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
            $"Class {pClass} does not contain member with name {memberName}.");
    }
    
    private void ParseFullStaticClassMember(string namespaceName)
    {
        // Consume scope
        _stream.Read();
        
        string className = _stream.ConsumeIdentifer();

        PrototypeClass pClass = _containingClass.ResolveFullClass(
            _stream.Location,
            namespaceName, className
        );

        // A class on its own is not a valid expression, so it always involves a member reference
        // e.g. Namespace::Class.function
        _stream.Consume(TokenType.Dot);

        string memberName = _stream.ConsumeIdentifer();

        AddStaticMemberReference(pClass, memberName);
    }

    private void ParseImplicitStaticClassMember(string className)
    {
        if (!_containingClass.TryResolveImplicitClass(_stream.Location, className, out PrototypeClass pClass))
        {
            ParseLocalIdentifier(className);
            return;
        }
        
        // Consume dot
        _stream.Read();
        
        string memberName = _stream.ConsumeIdentifer();
        
        AddStaticMemberReference(pClass, memberName);
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
        if (_functionParser != null
            && _functionParser.TryGetVariableLocation(identifier, out int location))
        {
            _expressionStack.Push(new ReferenceExpression(
                _stream.Location,
                ReferenceType.Local,
                location
            ));
            
            return;
        }

        bool isStatic = _functionParser == null
            || _functionParser.PrototypeFunction.IsStatic;
        
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
                    ReferenceType.StaticField,
                    field.AssemblyLocation
                ));
            }
            else
            {
                if (isStatic)
                {
                    throw new LangException(_stream,
                        $"Cannot reference instance field '{identifier}' in a static context.");
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

        if (false)
        {
            
        }

        throw new LangException(_stream,
            $"No local variable, member or class with name '{identifier}' found.");
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

    private void ParseConstructor(Token token)
    {
        string namespaceName = null;
        string className = _stream.ConsumeIdentifer();

        if (_stream.TryConsume(TokenType.Scope))
        {
            namespaceName = className;
            className = _stream.ConsumeIdentifer();
        }
        
        SealClass sClass = _containingClass.ResolveSealClass(
            _stream.Location,
            className,
            namespaceName
        );

        _stream.Consume(TokenType.OpenParen);

        Expression[] argumentExpressions = GetParsedArgumentList(TokenType.CloseParen);
        
        _expressionStack.Push(new ConstructorExpression(
            token.Location,
            sClass,
            argumentExpressions
        ));
    }

    private void ParseOpenSquare(Token token)
    {
        // Check for function call
        if (_stream.Position > 1 && IsCallable(_stream[_stream.Position - 2]))
        {
            ParseIndexExpression(token);
        }
        else
        {
            ParseArrayExpression(token);
        }
    }

    private void ParseIndexExpression(Token token)
    {
        FlushPrecedence(LangConfig.MaxPrecedence);
        
        PopUnary(token, out Expression instanceExpression);
        
        Expression[] argumentExpressions = GetParsedArgumentList(TokenType.CloseSquare);
        
        _expressionStack.Push(new IndexerExpression(
            token.Location,
            argumentExpressions,
            instanceExpression
        ));
    }

    private void ParseArrayExpression(Token token)
    {
        Expression[] itemExpressions = GetParsedArgumentList(TokenType.CloseSquare);
        
        _expressionStack.Push(new ArrayExpression(
            token.Location,
            itemExpressions
        ));
    }

    private Dictionary<Expression, Expression> GetParsedExpressionMap()
    {
        if (_stream.TryConsume(TokenType.CloseBrace))
            return [];

        ExpressionParser parser = CreateSubParser(ExpressionParsingMode.Argument);

        var items = new Dictionary<Expression, Expression>();
        
        while (!_stream.EndOfStream)
        {
            Expression key = parser.Parse();
            
            _stream.Consume(TokenType.Colon);
            
            Expression value = parser.Parse();
            
            // this should not fail as each expression is unique
            items.Add(key, value);

            if (_stream.Peek().TokenType == TokenType.CloseBrace)
                break;

            _stream.Consume(TokenType.Comma);
            
            // Allow trailing comma
            if (_stream.Peek().TokenType == TokenType.CloseBrace)
                break;
        }

        _stream.Consume(TokenType.CloseBrace);

        return items;
    }

    private void ParseMapExpression(Token token)
    {
        Dictionary<Expression, Expression> itemExpressions = GetParsedExpressionMap();
        
        _expressionStack.Push(new MapExpression(
            token.Location,
            itemExpressions
        ));
    }
    
    private void TransferOperator()
    {
        Token token = _operatorStack.Pop();

        switch (token.TokenType)
        {
        // Arithmetic
        case TokenType.Power:
        case TokenType.Multiply:
        case TokenType.Divide:
        case TokenType.IDivide:
        case TokenType.Modulo:
        case TokenType.Add:
        case TokenType.Subtract:
        case TokenType.And:
        case TokenType.Xor:
        case TokenType.Or:
            ParseArithmeticExpression(token);
            break;
        // Compound Arithmetic
        case TokenType.PowerAssign:
        case TokenType.MultiplyAssign:
        case TokenType.DivideAssign:
        case TokenType.IDivideAssign:
        case TokenType.ModuloAssign:
        case TokenType.AddAssign:
        case TokenType.SubtractAssign:
        case TokenType.AndAssign:
        case TokenType.XorAssign:
        case TokenType.OrAssign:
            ParseCompoundArithmeticExpression(token);
            break;
        // Comparison
        case TokenType.LessThan:
        case TokenType.GreaterThan:
        case TokenType.LessThanOrEqual:
        case TokenType.GreaterThanOrEqual:
        case TokenType.Equal:
        case TokenType.NotEqual:
            ParseComparisonExpression(token);
            break;
        // Unary
        case TokenType.Minus:
        case TokenType.Not:
        case TokenType.Typeof:
            ParseUnaryExpression(token);
            break;
        // Other
        case TokenType.Assign:
            ParseAssignExpression(token);
            break;
        case TokenType.ConditionalAnd:
            ParseConditionalAndExpression(token);
            break;
        case TokenType.ConditionalOr:
            ParseConditionalOrExpression(token);
            break;
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

    private void ParseArithmeticExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
            
        _expressionStack.Push(new ArithmeticExpression(
            token.Location,
            token.TokenType,
            left,
            right
        ));
    }
    
    private void ParseCompoundArithmeticExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);

        if (left is not AssignableExpression leftAssignable)
        {
            throw new LangException(token,
                $"Compound operator {token.TokenType} expected left-hand side to be assignable, got {left}.");
        }
            
        _expressionStack.Push(new CompoundArithmeticExpression(
            token.Location,
            token.TokenType,
            leftAssignable,
            right
        ));
    }
    
    private void ParseComparisonExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        
        _expressionStack.Push(new ComparisonExpression(
            token.Location,
            token.TokenType,
            left,
            right
        ));
    }
    
    private void ParseUnaryExpression(Token token)
    {
        PopUnary(token, out Expression operand);
        
        _expressionStack.Push(new UnaryExpression(
            token.Location,
            token.TokenType,
            operand
        ));
    }

    private void ParseAssignExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);

        if (left is not AssignableExpression assignable)
            throw new LangException(token,
                $"Assignment expected left-hand side to be assignable, got {left}.");
        
        _expressionStack.Push(new AssignExpression(
            token.Location,
            assignable,
            right
        ));
    }

    private void ParseConditionalAndExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        
        _expressionStack.Push(new ConditionalAndExpression(
            token.Location,
            left,
            right
        ));
    }
    
    private void ParseConditionalOrExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        
        _expressionStack.Push(new ConditionalOrExpression(
            token.Location,
            left,
            right
        ));
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
        while (_operatorStack.TryPeek(out Token token))
        {
            if (token.TokenType == TokenType.OpenParen)
                throw new LangException(token,
                    "Open parenthesis without matching close parenthesis.");
            
            TransferOperator();
        }
    }
}