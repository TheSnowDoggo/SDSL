using System.Diagnostics.CodeAnalysis;

namespace SDSL.Expressions;

public class ExpressionParser
{
	private readonly VariantAssembly _assembly;
	private readonly VariantClass _class;
	private readonly FunctionParser _functionParser;
	
	private readonly ExpressionParsingMode _parsingMode;

	private readonly TokenStream _stream;
	
	private readonly Stack<Token> _operatorStack = [];
    private readonly Stack<Expression> _expressionStack = [];

    private int _bracketDepth;
    
    public ExpressionParser(
	    VariantAssembly assembly,
	    VariantClass variantClass,
	    [AllowNull] FunctionParser functionParser,
	    ExpressionParsingMode parsingMode,
	    TokenStream stream)
    {
	    _assembly = assembly;
	    _class = variantClass;
	    _functionParser = functionParser;
	    _parsingMode = parsingMode;
	    _stream = stream;
    }

    public Expression Parse()
    {
	    _operatorStack.Clear();
	    _expressionStack.Clear();

	    _bracketDepth = 0;

	    ParseExpressions();

	    switch (_expressionStack.Count)
	    {
	    case 0:
		    throw new ParserException(_stream, "Expression was empty.");
	    case 1:
		    return _expressionStack.Pop();
	    default:
		    throw new ParserException(_stream,
			    $"Expression stack contained multiple items: [{string.Join(", ", _expressionStack)}]");
	    }
    }

    public Expression[] ParseFullArgumentList()
    {
	    _stream.Consume(TokenType.OpenParen);

	    return GetParsedArgumentList(TokenType.CloseParen);
    }
    
    private static bool IsCallable(Token token)
    {
	    return token.TokenType is TokenType.Identifier
		    or TokenType.Literal
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
        
	    int otherPrecedence = GlobalMaps.PrecedenceMap[other.TokenType];

	    if (precedence < otherPrecedence)
	    {
		    return true;
	    }

	    if (precedence > otherPrecedence)
	    {
		    return false;
	    }

	    // If both operators are right associative and the precedence is the same, do not flush.
	    if (GlobalMaps.RightAssociativeSet.Contains(token.TokenType)
	        && GlobalMaps.RightAssociativeSet.Contains(other.TokenType))
	    {
		    return false;
	    }

	    return true;
    }
    
    private void ParseExpressions()
    {
	    while (!_stream.EndOfStream)
	    {
		    Token token = _stream.Peek();

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
			    PushExpression(new ValueExpression(token.Value));
			    break;
		    case TokenType.Dot:
			    ParseMemberExpression(token);
			    break;
		    case TokenType.New:
			    ParseConstructor();
			    break;
		    case TokenType.OpenSquare:
			    ParseOpenSquare(token);
			    break;
		    case TokenType.OpenBrace:
			    ParseMapExpression();
			    break;
		    case TokenType.Base:
			    ParseBaseExpression();
			    break;
		    default:
			    PushOperator(token);
			    break;
		    }
	    }
	    
	    FlushAll();
    }
    
    private bool ShouldExit(Token token)
    {
	    return _parsingMode switch
	    {
		    ExpressionParsingMode.Statement 
			    => token.TokenType is TokenType.Semicolon
				       or TokenType.CloseBrace,
		    ExpressionParsingMode.Argument 
			    => token.TokenType is TokenType.Comma
				       or TokenType.CloseSquare
				       or TokenType.CloseBrace
				       or TokenType.Colon
			       || (_bracketDepth == 0 && token.TokenType is TokenType.CloseParen),
		    ExpressionParsingMode.Condition
			    => token.TokenType is TokenType.OpenBrace,
		    _ => throw new InvalidOperationException(
			    $"Had invalid parsing mode: {_parsingMode}."),
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
        FlushPrecedence(GlobalMaps.MaxPrecedence);
        
        PopUnary(token, out Expression expression);

        Expression[] argumentList = GetParsedArgumentList(TokenType.CloseParen);
        
        if (expression is IMemberFunctionExpression functionExpression)
        {
	        PushExpression(new MemberInvokeExpression(functionExpression, argumentList));
        }
        else
        {
	        PushExpression(new StaticInvokeExpression(expression, argumentList));
        }
    }

    private ExpressionParser CreateSubParser(ExpressionParsingMode parsingMode)
    {
        return new ExpressionParser(_assembly, _class, _functionParser, parsingMode, _stream);
    }

    private Expression[] GetParsedArgumentList(TokenType closeType, bool allowTrailingComma = false)
    {
        if (_stream.TryConsume(closeType))
        {
            return [];
        }

        ExpressionParser parser = CreateSubParser(ExpressionParsingMode.Argument);

        var arguments = new List<Expression>();
        
        while (!_stream.EndOfStream)
        {
            arguments.Add(parser.Parse());

            if (_stream.Peek().TokenType == closeType)
            {
                break;
            }

            _stream.Consume(TokenType.Comma);

            if (allowTrailingComma && _stream.Peek().TokenType == closeType)
            {
                break;
            }
        }

        _stream.Consume(closeType);

        return arguments.ToArray();
    }

    private void ParseCloseParen(Token token)
    {
        if (_bracketDepth == 0)
        {
            throw new ParserException(token, "No matching open parenthesis found for close parenthesis.");
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
        // Implicit Static Function/Field ref OR Local Variable
        case TokenType.Dot:
            ParseStaticClassMember(identifer);
            break;
        // Local Variable
        default:
            ParseLocalIdentifier(identifer);
            break;
        }
    }
    
    private void AddStaticMemberReference(VariantClass variantClass, string memberName)
    {
        if (variantClass.FunctionMap.TryGetValue(memberName, out Function function))
        {
            if (!function.IsStatic)
            {
                throw new ParserException(_stream,
                    $"Cannot reference member function '{function.FullName}' in a static context.");
            }
            
            PushExpression(new ValueExpression(function));

            return;
        }

        if (variantClass.PropertyMap.TryGetValue(memberName, out Property property))
        {
	        if (!property.IsStatic)
	        {
		        throw new ParserException(_stream,
			        $"Cannot reference member property '{property.FullName}' in a static context.");
	        }
	        
	        PushExpression(new StaticPropertyExpression(property));
	        
	        return;
        }

        if (variantClass.ConstantMap.TryGetValue(memberName, out Constant constant))
        {
            PushExpression(new ValueExpression(constant.Value));
            
            return;
        }

        throw new ParserException(_stream,
            $"Class {variantClass} does not contain member '{memberName}'.");
    }
    
    private void ParseStaticClassMember(string className)
    {
        if (!_assembly.Classes.TryGetValue(className, out VariantClass variantClass))
        {
            ParseLocalIdentifier(className);
            return;
        }
        
        // Consume dot
        _stream.Read();
        
        string memberName = _stream.ConsumeIdentifer();
        
        AddStaticMemberReference(variantClass, memberName);
    }
    
    private bool TryAddImplicitReference(
        VariantClass variantClass,
        string identifier,
        bool isStatic)
    {
        if (variantClass.FunctionMap.TryGetValue(identifier,
            out Function function))
        {
            if (function.IsStatic)
            {
                // Implicit Class.static_function
                PushExpression(new ValueExpression(function));
            }
            else
            {
                if (isStatic)
                {
                    throw new ParserException(_stream,
                        $"Cannot reference instance function '{identifier}' in a static context.");
                }
                
                // Implicit self.instance_function
                PushExpression(new FixedInstanceFunctionExpression(LocalRefExpression.Self, function));
            }
            
            return true;
        }

        if (variantClass.PropertyMap.TryGetValue(identifier,
                out Property property))
        {
            if (property.IsStatic)
            {
                // Implicit Class.static_field
                PushExpression(new StaticPropertyExpression(property));
            }
            else
            {
                if (isStatic)
                {
                    throw new ParserException(_stream,
                        $"Cannot reference member field '{identifier}' in a static context.");
                }
                
                // Implicit self.instance_field
                PushExpression(new FixedInstancePropertyExpression(LocalRefExpression.Self, property));
            }
            
            return true;
        }

        if (variantClass.ConstantMap.TryGetValue(identifier, out Constant constant))
        {
            PushExpression(new ValueExpression(constant.Value));
            
            return true;
        }

        return false;
    }

    private void ParseLocalIdentifier(string identifier)
    {
        // Checks are in order of shadowing priority

        // Is local variable?
        if (_functionParser != null
            && _functionParser.Allocator.TryGetVariableLocation(identifier, out int location))
        {
            PushExpression(new LocalRefExpression(location, identifier));
            
            return;
        }

        bool isStatic = _functionParser == null || _functionParser.Function.IsStatic;

        // Implicit references in containing class
        if (TryAddImplicitReference(_class, identifier, isStatic))
        {
            return;
        }

        if (TryAddImplicitReference(GlobalClass.Class, identifier, true))
        {
	        return;
        }

        throw new ParserException(_stream,
            $"No local/global variable, member or class with name '{identifier}' found.");
    }

    private void ParseMemberExpression(Token token)
    {
        FlushPrecedence(GlobalMaps.MaxPrecedence);
        
        PopUnary(token, out Expression selfExpression);

        string identifier = _stream.ConsumeIdentifer();
        
        PushExpression(new MemberExpression(selfExpression, identifier));
    }

    private void ParseConstructor()
    {
	    Token classToken = _stream.Consume(TokenType.Identifier);
        string className = classToken.Value.AsString();

        if (!_assembly.Classes.TryGetValue(className, out VariantClass variantClass))
        {
	        throw new ParserException(classToken,
		        $"Failed to create constructor call : No class with name '{className}' found.");
        }

        _stream.Consume(TokenType.OpenParen);

        Expression[] argumentExpressions = GetParsedArgumentList(TokenType.CloseParen);
        
        PushExpression(new ConstructorExpression(
            variantClass,
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
            ParseArrayExpression();
        }
    }

    private void ParseIndexExpression(Token token)
    {
        FlushPrecedence(GlobalMaps.MaxPrecedence);
        
        PopUnary(token, out Expression instanceExpression);
        
        Expression[] argumentExpressions = GetParsedArgumentList(TokenType.CloseSquare);
        
        PushExpression(new IndexerExpression(
            argumentExpressions,
            instanceExpression
        ));
    }

    private void ParseArrayExpression()
    {
        Expression[] itemExpressions = GetParsedArgumentList(TokenType.CloseSquare, allowTrailingComma: true);
        
        PushExpression(new ArrayExpression(itemExpressions));
    }

    private Dictionary<Expression, Expression> GetParsedExpressionMap()
    {
        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            return [];
        }

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
            {
                break;
            }

            _stream.Consume(TokenType.Comma);
            
            // Allow trailing comma
            if (_stream.Peek().TokenType == TokenType.CloseBrace)
            {
                break;
            }
        }

        _stream.Consume(TokenType.CloseBrace);

        return items;
    }

    private void ParseMapExpression()
    {
        Dictionary<Expression, Expression> itemExpressions = GetParsedExpressionMap();
        
        PushExpression(new MapExpression(itemExpressions));
    }

    private void ParseBaseExpression()
    {
	    if (_functionParser == null || _functionParser.Function.IsStatic)
	    {
		    throw new ParserException(_stream,
			    "Cannot call base function in static context.");
	    }

	    VariantClass baseClass = _class.BaseClass;
	    
	    if (baseClass == null)
	    {
		    throw new ParserException(_stream,
			    "Cannot call base function as class has no base class.");
	    }
	    
	    FlushPrecedence(GlobalMaps.MaxPrecedence);

	    _stream.Consume(TokenType.Dot);

	    Token nameToken = _stream.Consume(TokenType.Identifier);
	    string name = nameToken.Value.AsString();

	    if (!baseClass.FunctionMap.TryGetValue(name, out Function function))
	    {
		    throw new ParserException(nameToken,
			    $"Base class {baseClass} does not contain function '{name}'.");
	    }

	    if (function.IsStatic)
	    {
		    throw new ParserException(nameToken,
			    $"Cannot base call static function {function.FullName}.");
	    }

	    _stream.Consume(TokenType.OpenParen);

	    Expression[] argumentList = GetParsedArgumentList(TokenType.CloseParen);
	    
	    PushExpression(new MemberInvokeExpression(
		    new FixedInstanceFunctionExpression(
			    LocalRefExpression.Self,
			    function
			),
		    argumentList
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
            case TokenType.ShiftLeft:
            case TokenType.ShiftRight:
            case TokenType.ShiftRightU:
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
            case TokenType.ShiftLeftAssign:
            case TokenType.ShiftRightAssign:
            case TokenType.ShiftRightUAssign:
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
            case TokenType.Plus:
            case TokenType.Not:
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
                throw new ParserException(token,
                    $"Cannot create expression for operator {token.TokenType}.");
        }
    }
    
    private void PopUnary(
        Token token,
        out Expression operand)
    {
        if (_expressionStack.Count < 1)
        {
            throw new ParserException(token,
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
            throw new ParserException(token,
                $"{token.TokenType} expected 2 operands, got {_expressionStack.Count}.");
        }
        
        right = _expressionStack.Pop();
        left = _expressionStack.Pop();
    }

    private void ParseArithmeticExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
            
        PushExpression(new ArithmeticExpression(token.TokenType, left, right));
    }
    
    private void ParseCompoundArithmeticExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);

        if (left is not AssignableExpression assignable)
        {
            throw new ParserException(token,
                $"Compound operator {token.TokenType} expected left-hand side to be assignable, got {left}.");
        }
        
        PushExpression(new CompoundArithmeticExpression(token.TokenType, assignable, right));
    }
    
    private void ParseComparisonExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        
        PushExpression(new ComparisonExpression(token.TokenType, left, right));
    }
    
    private void ParseUnaryExpression(Token token)
    {
        PopUnary(token, out Expression operand);
        
        PushExpression(new UnaryExpression(token.TokenType, operand));
    }

    private void ParseAssignExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);

        if (left is not AssignableExpression assignable)
        {
            throw new ParserException(token,
                $"Assignment expected left-hand side to be assignable, got {left}.");
        }

        PushExpression(new AssignExpression(assignable, right));
    }
    
    private void ParseConditionalAndExpression(Token token)
    {
	    PopBinary(token, out Expression left, out Expression right);

	    PushExpression(new ConditionalAndExpression(left, right));
    }
    
    private void ParseConditionalOrExpression(Token token)
    {
	    PopBinary(token, out Expression left, out Expression right);

	    PushExpression(new ConditionalOrExpression(left, right));
    }

    private void PushExpression(Expression expression)
    {
	    // Constant evaluation optimisation
	    if (expression.IsConstantEval() && expression is not ValueExpression)
	    {
		    expression = new ValueExpression(expression.Evaluate(null));
	    }
        
	    _expressionStack.Push(expression);
    }
    
    private void PushOperator(Token token)
    {
	    // Try convert to an associated unary operator (such as for subtract/minus)
	    if (GlobalMaps.UnaryMap.TryGetValue(token.TokenType, out TokenType unaryType)
	        && (_stream.Position == 0 || !IsOperand(_stream[_stream.Position - 2])))
	    {
		    token.TokenType = unaryType;
	    }
        
	    // Most likely occurs from reading past the expression when a semicolon is missed
	    if (!GlobalMaps.PrecedenceMap.TryGetValue(token.TokenType, out int precedence))
	    {
		    throw new ParserException(_stream, 
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
	           && GlobalMaps.PrecedenceMap[other.TokenType] >= precedence)
	    {
		    TransferOperator();
	    }
    }
    
    private void FlushAll()
    {
	    while (_operatorStack.TryPeek(out Token token))
	    {
		    if (token.TokenType is TokenType.OpenParen)
		    {
			    throw new ParserException(token,
				    "Tried to flush open parenthesis without matching close parenthesis.");
		    }
            
		    TransferOperator();
	    }
    }
}