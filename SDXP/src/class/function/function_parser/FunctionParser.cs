using System.Collections.Frozen;
using SDSL.Expressions;
using SDSL.Statements;

namespace SDSL;

public class FunctionParser
{
	public const string SelfName = "self";
	
	private readonly VariantAssembly _assembly;
	private readonly TokenStream _stream;
	
	public FunctionParser(
		VariantAssembly assembly,
		UserFunction function)
	{
		_assembly = assembly;
		Function = function;
		
		_stream = new TokenStream(function.Tokens);

		Allocator = new VariableAllocator(_stream);
	}
	
	public UserFunction Function { get; }
	public VariableAllocator Allocator { get; }

	public void Parse()
	{
		DefineArguments();

		ParseStatements();
	}
	
	public void DefineArguments()
	{
		Allocator.OpenScope();
		
		if (!Function.IsStatic)
		{
			Allocator.DefineVariable(SelfName);
		}
		
		FunctionArgument[] arguments = Function.Signature.Arguments;

		for (int i = 0; i < arguments.Length; i++)
		{
			Allocator.DefineVariable(arguments[i].Name);
		}
	}

	public void ParseStatements()
	{
		var statements = new List<Statement>();
        
		while (!_stream.EndOfStream)
		{
			statements.Add(ParseStatement());
		}

		Function.Statements = statements.ToArray();
		Function.VariableCount = Allocator.VariableCount;
		Function.Tokens = ArraySegment<Token>.Empty;
	}

	private Statement ParseStatement()
	{
		Token head = _stream.Peek();

		return head.TokenType switch
		{
			TokenType.Var
				=> ParseDefineStatement(),
			TokenType.Identifier or TokenType.New or TokenType.Base
				=> ParseExpressionStatement(),
			TokenType.Return
				=> ParseReturnStatement(),
			TokenType.OpenBrace
				=> ParseBlockStatement(),
			TokenType.If
				=> ParseIfStatement(),
			TokenType.While
				=> ParseWhileStatement(),
			TokenType.Break
				=> ParseControlStatement(ReturnValue.Break),
			TokenType.Continue
				=> ParseControlStatement(ReturnValue.Continue),
			TokenType.For
				=> ParseForStatement(),
			TokenType.Switch
				=> ParseSwitchStatement(),
			_ => throw new ParserException(head.Location, $"Got unexpected token {head.TokenType} parsing statement."),
		};
	}

	private DefineStatement ParseDefineStatement()
	{
		// Consume var
		Token head = _stream.Read();

		string name = _stream.ConsumeIdentifer();
		
		VariantClass variantClass;
		Expression expression;
		
		if (_stream.TryConsume(TokenType.TypeAssign))
		{
			variantClass = IncompleteClass.Implicit;
			expression = ParseExpression(ExpressionParsingMode.Statement);
		}
		else
		{
			variantClass = ParseTypeAnnotation();

			expression = _stream.TryConsume(TokenType.Assign)
				? ParseExpression(ExpressionParsingMode.Statement)
				: new ValueExpression(VariantClass.GetDefaultValue(variantClass));
		}

		// define variable after parsing expression to avoid self reference
		
		int refLocation = Allocator.DefineVariable(name);
		
		ConsumeTerminator();

		return new DefineStatement(
			head.Location,
			refLocation,
			name,
			variantClass,
			expression
		);
	}
	
	private ExpressionStatement ParseExpressionStatement()
	{
		// Do not consume
		Token head = _stream.Peek();

		Expression expression = ParseExpression(ExpressionParsingMode.Statement);
		
		ConsumeTerminator();

		return new ExpressionStatement(
			head.Location,
			expression
		);
	}

	private ReturnStatement ParseReturnStatement()
	{
		// Consume return
		Token head = _stream.Read();

		if (_stream.TryConsume(TokenType.Semicolon))
		{
			return new ReturnStatement(head.Location, ValueExpression.Nil);
		}
		
		Expression expression = ParseExpression(ExpressionParsingMode.Statement);

		ConsumeTerminator();

		return new ReturnStatement(
			head.Location,
			expression
		);
	}

	private BlockStatement ParseBlockStatement()
	{
		// Do not consume {
		Token head = _stream.Peek();

		Statement[] statements = ParseStatementBlock();

		return new BlockStatement(
			head.Location,
			statements
		);
	}

	private IfStatement ParseIfStatement()
    {
        // Consume if
        Token head = _stream.Read();

        Expression condition = ParseExpression(ExpressionParsingMode.Condition);

        Statement[] statements = ParseStatementBlock();

        BlockStatement elseBlock = null;

        if (_stream.TryConsume(TokenType.Else))
        {
	        elseBlock = _stream.Peek().TokenType == TokenType.If
		        ? ParseIfStatement()
		        : ParseBlockStatement();
        }

        return new IfStatement(
            head.Location,
            statements,
            condition,
            elseBlock
        );
    }

    private WhileStatement ParseWhileStatement()
    {
        // Consume while
        Token head = _stream.Read();
        
        Expression condition = ParseExpression(ExpressionParsingMode.Condition);
        
        Statement[] statements = ParseStatementBlock();

        return new WhileStatement(
            head.Location,
            statements,
            condition
        );
    }

    private ControlStatement ParseControlStatement(ReturnValue returnValue)
    {
        Token head = _stream.Read();

        ConsumeTerminator();

        return new ControlStatement(
            head.Location,
            returnValue
        );
    }

    private ForStatement ParseForStatement()
    {
        // Consume for
        Token head = _stream.Read();

        string identifier = _stream.ConsumeIdentifer();
        
        // The identifier exists inside the loop scope
        Allocator.OpenScope();
        
        VariantClass variableClass = ParseTypeAnnotation();
        
        int variableLocation = Allocator.DefineVariable(identifier);

        _stream.Consume(TokenType.In);

        Expression expression = ParseExpression(ExpressionParsingMode.Condition);

        Statement[] statements = ParseStatementBlock(openScope: false);

        return new ForStatement(
            head.Location,
            statements,
            variableLocation,
            variableClass,
            expression
        );
    }

    private List<Variant> ParseSwitchCaseValues()
    {
        if (_stream.TryConsume(TokenType.Default))
        {
            _stream.Consume(TokenType.Colon);
            return null;
        }

        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            throw new ParserException(_stream,
                "Switch case must have at least one value.");
        }

        ExpressionParser parser = CreateExpressionParser(ExpressionParsingMode.Argument);
        
        var values = new List<Variant>();

        while (!_stream.EndOfStream)
        {
	        Token head = _stream.Peek();
	        
            Expression expression = parser.Parse();

            if (!expression.IsConstantEval())
            {
                throw new ParserException(head,
                    "Switch case expression was not evaluatable in a constant context.");
            }

            Variant value = expression.Evaluate(null);
            
            values.Add(value);
            
            // Always end in colon
            _stream.Consume(TokenType.Colon);

            if (_stream.Peek().TokenType == TokenType.OpenBrace)
            {
                break;
            }
        }
        
        return values;
    }

    private (FrozenDictionary<Variant, BlockStatement>, BlockStatement) ParseSwitchBlocks()
    {
        _stream.Consume(TokenType.OpenBrace);

        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            return (FrozenDictionary<Variant, BlockStatement>.Empty, null);
        }

        var blocks = new Dictionary<Variant, BlockStatement>();

        BlockStatement defaultBlock = null;

        while (!_stream.EndOfStream)
        {
            List<Variant> values = ParseSwitchCaseValues();

            BlockStatement block = ParseBlockStatement();

            if (values == null)
            {
                if (defaultBlock != null)
                {
                    throw new ParserException(block.Location,
                        "Switch statment contained multiple default blocks.");
                }

                defaultBlock = block;
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    Variant value = values[i];
                
                    if (!blocks.TryAdd(value, block))
                    {
                        throw new ParserException(_stream,
                            $"Switch case had duplicate value {value}.");
                    }
                }
            }

            if (_stream.Peek().TokenType == TokenType.CloseBrace)
            {
                break;
            }
        }

        _stream.Consume(TokenType.CloseBrace);

        return (blocks.ToFrozenDictionary(), defaultBlock);
    }

    private SwitchStatement ParseSwitchStatement()
    {
        // consume switch
        Token head = _stream.Read();

        Expression expression = CreateExpressionParser(ExpressionParsingMode.Condition).Parse();

        (FrozenDictionary<Variant, BlockStatement> blocks, BlockStatement defaultBlock) = ParseSwitchBlocks();

        return new SwitchStatement(
            head.Location,
            expression,
            blocks,
            defaultBlock
        );
    }
    
    private ExpressionParser CreateExpressionParser(ExpressionParsingMode parsingMode)
    {
	    return new ExpressionParser(_assembly, Function.LocalClass, this, parsingMode, _stream);
    }
	
	private Expression ParseExpression(ExpressionParsingMode parsingMode)
	{
		return CreateExpressionParser(parsingMode).Parse();
	}
	
	private VariantClass ParseTypeAnnotation()
	{
		if (!_stream.TryConsume(TokenType.Colon))
		{
			return null;
		}

		Token classToken = _stream.Consume(TokenType.Identifier);
		string className = classToken.Value.AsString();

		if (!_assembly.Classes.TryGetValue(className, out VariantClass variantClass))
		{
			throw new ParserException(classToken,
				$"Class with name '{className}' not found.");
		}

		return variantClass;
	}

	private Statement[] ParseStatementBlock(bool openScope = true)
	{
		_stream.Consume(TokenType.OpenBrace);

		if (_stream.TryConsume(TokenType.CloseBrace))
		{
			return [];
		}

		if (openScope)
		{
			Allocator.OpenScope();
		}

		var statements = new List<Statement>();

		while (!_stream.EndOfStream)
		{
			statements.Add(ParseStatement());

			if (_stream.Peek().TokenType == TokenType.CloseBrace)
			{
				break;
			}
		}

		_stream.Consume(TokenType.CloseBrace);
		
		if (openScope)
		{
			Allocator.CloseScope();
		}

		return statements.ToArray();
	}

	private void ConsumeTerminator()
	{
		_stream.Consume(TokenType.Semicolon);
	}
}