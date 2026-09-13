using SDSL.Expressions;
using SDSL.Statements;

namespace SDSL;

public class FunctionParser
{
	public const string SelfName = "self";
	
	private readonly VariantAssembly _assembly;
	private readonly UserFunction _function;
	private readonly TokenStream _stream;

	private readonly VariableAllocator _allocator;

	public FunctionParser(
		VariantAssembly assembly,
		UserFunction function,
		TokenStream stream)
	{
		_assembly = assembly;
		_function = function;
		_stream = stream;

		_allocator = new VariableAllocator(_stream);
	}

	public void Parse()
	{
		_allocator.OpenScope();

		DefineArguments();
		
		var statements = new List<Statement>();
        
		while (!_stream.EndOfStream)
		{
			statements.Add(ParseStatement());
		}
	}

	private void DefineArguments()
	{
		if (!_function.IsStatic)
		{
			_allocator.DefineVariable(SelfName);
		}
		
		FunctionArgument[] arguments = _function.Signature.Arguments;

		for (int i = 0; i < arguments.Length; i++)
		{
			_allocator.DefineVariable(arguments[i].Name);
		}
	}

	private Statement ParseStatement()
	{
		Token head = _stream.Peek();

		return head.TokenType switch
		{
			TokenType.Var
				=> ParseDefineStatement(),
			TokenType.Identifier or TokenType.New
				=> ParseExpressionStatement(),
			TokenType.Return
				=> ParseReturnStatement(),
			TokenType.OpenBrace
				=> ParseBlockStatement(),
			TokenType.If
				=> ParseIfStatement(),
			TokenType.While
				=> throw new NotImplementedException(),
			TokenType.Break
				=> throw new NotImplementedException(),
			TokenType.Continue
				=> throw new NotImplementedException(),
			TokenType.For
				=> throw new NotImplementedException(),
			TokenType.Switch
				=> throw new NotImplementedException(),
			_ => throw new ParserException(head.Location, $"Got unexpected token {head.TokenType} parsing statement."),
		};
	}

	private DefineStatement ParseDefineStatement()
	{
		// Consume var
		Token head = _stream.Read();

		string name = _stream.ConsumeIdentifer();

		int refLocation = _allocator.DefineVariable(name);

		VariantClass variantClass;
		Expression expression;
		
		if (_stream.TryConsume(TokenType.TypeAssign))
		{
			variantClass = ImplicitVariantClass.Instance;
			expression = ParseExpression();
		}
		else
		{
			variantClass = ParseTypeAnnotation();

			expression = _stream.TryConsume(TokenType.Assign)
				? ParseExpression()
				: new ValueExpression(_stream.Location, VariantClass.GetDefaultValue(variantClass));
		}
		
		ConsumeTerminator();

		return new DefineStatement(
			head.Location,
			refLocation,
			variantClass,
			expression
		);
	}
	
	private ExpressionStatement ParseExpressionStatement()
	{
		// Do not consume
		Token head = _stream.Peek();

		Expression expression = ParseExpression();
		
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

		Expression expression = ParseExpression();

		ConsumeTerminator();

		return new ReturnStatement(
			head.Location,
			expression
		);
	}

	private BlockStatement ParseBlockStatement()
	{
		// Consume {
		Token head = _stream.Read();

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

		throw new NotImplementedException();
	}
	
	private Expression ParseExpression()
	{
		throw new NotImplementedException();
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
			_allocator.OpenScope();
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
			_allocator.CloseScope();
		}

		return statements.ToArray();
	}

	private void ConsumeTerminator()
	{
		_stream.Consume(TokenType.Semicolon);
	}
}