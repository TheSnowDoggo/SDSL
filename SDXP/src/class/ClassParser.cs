using SDSL.Expressions;
using SDSL.Native;

namespace SDSL;

public class ClassParser
{
	private readonly VariantAssembly _assembly;
	private readonly TokenStream _stream;

	private UserClass _userClass;
	
	public ClassParser(VariantAssembly assembly, TokenStream stream)
	{
		_assembly = assembly;
		_stream = stream;
	}

	public static void ParseDirectory(VariantAssembly assembly, string directory)
	{
		foreach (string file in Directory.EnumerateFiles(
			directory, "*.sdsl", SearchOption.AllDirectories))
		{
			Token[] tokens;

			string sourceName = Path.GetRelativePath(directory, file);

			using (Tokenizer tokenizer = new Tokenizer(File.OpenText(file), sourceName))
			{
				tokens = tokenizer.Tokenize();
			}

			TokenStream stream = new TokenStream(tokens);
		
			new ClassParser(assembly, stream).Parse();
		}
	}

	public void Parse()
	{
		while (!_stream.EndOfStream)
		{
			Token head = _stream.Read();

			switch (head.TokenType)
			{
			case TokenType.Class:
				ParseClass();
				break;
			case TokenType.Enum:
				ParseEnum();
				break;
			default:
				throw new ParserException(head,
					$"Got unexpected token {head.TokenType} in class definition.");
			}
		}
	}

	private void ParseClass()
	{
		CreateClass();

		ParseBaseClass();

		ParseClassMembers();

		_userClass.UserConstructor ??= new UserConstructor(_userClass, null);
	}
	
	private void CreateClass()
	{
		Token nameToken = _stream.Consume(TokenType.Identifier);
		string className = nameToken.Value.AsString();
		
		_userClass = new UserClass(className);

		if (!_assembly.Classes.TryAdd(className, _userClass))
		{
			throw new ParserException(nameToken,
				$"Class with name {className} has already been defined.");
		}

		_assembly.UserClasses.Add(_userClass);
	}

	private void ParseBaseClass()
	{
		if (!_stream.TryConsume(TokenType.Colon))
		{
			return;
		}

		string identifier = _stream.ConsumeIdentifer();

		_userClass.PrototypeBaseClass = identifier;
	}

	private void ParseClassMembers()
	{
		_stream.Consume(TokenType.OpenBrace);

		while (!_stream.EndOfStream)
		{
			Token head = _stream.Peek();
			
			if (head.TokenType == TokenType.CloseBrace)
			{
				break;
			}
			
			bool isStatic = false;
			
			if (head.TokenType == TokenType.Static)
			{
				_stream.Advance();
				head = _stream.Peek();
				
				isStatic = true;
			}
			
			switch (head.TokenType)
			{
			// Field definition
			case TokenType.Var:
				ParseField(isStatic);
				break;
			// Function declaration
			case TokenType.Func:
				ParseFunction(isStatic);
				break;
			// Constant definition
			case TokenType.Const:
				if (isStatic)
				{
					throw new ParserException(_stream,
						$"User Class {_userClass} : Static qualifier is disallowed for constant definition.");
				}
				
				ParseConstant();
				break;
			// Constructor definition
			case TokenType.New:
				if (isStatic)
				{
					throw new ParserException(_stream,
						$"User Class {_userClass} : Static qualifier is disallowed for constructor definition.");
				}
				
				ParseConstructor();
				break;
			default:
				throw new ParserException(head,
					$"User Class {_userClass} : Got unexpected token {head} in class definition.");
			}
		}

		_stream.Consume(TokenType.CloseBrace);
	}

	private void RegisterMemberName(string name)
	{
		if (!_userClass.MemberNames.Add(name))
		{
			throw new ParserException(_stream,
				$"User Class {_userClass} : A duplicate member with name '{name}' has already been defined.");
		}
	}
	
	private void ParseField(bool isStatic)
	{
		// Consume var
		Token head = _stream.Read();
		
		string name = _stream.ConsumeIdentifer();

		RegisterMemberName(name);

		string pValueClass = _stream.TryConsume(TokenType.Colon)
			? _stream.ConsumeIdentifer()
			: null;

		ArraySegment<Token> tokens = _stream.TryConsume(TokenType.Assign)
			? GetStatementTokens(TokenType.Semicolon)
			: ArraySegment<Token>.Empty;

		ConsumeTerminator();

		Property property;
		
		if (isStatic)
		{
			property = new UserStaticProperty(name, _userClass, pValueClass, head.Location, tokens);
		}
		else
		{
			property = new UserInstanceProperty(name, _userClass, pValueClass, head.Location, tokens);
		}
		
		_userClass.DeclaredProperties.Add(property);
	}
	
	private void ParseFunction(bool isStatic)
	{
		Token head = _stream.Read();
		
		string name = _stream.ConsumeIdentifer();

		RegisterMemberName(name);

		(FunctionSignature signature, Variant[] defaultValues) = ParseFunctionSignature(true);

		ArraySegment<Token> tokens = GetFunctionBodyTokens();

		UserFunction function = new UserFunction(
			name,
			_userClass,
			isStatic,
			signature,
			defaultValues,
			head.Location,
			tokens
		);
		
		_userClass.DeclaredFunctions.Add(function);
	}

	private void ParseConstant()
	{
		// Consume const
		Token head = _stream.Read();
		
		string name = _stream.ConsumeIdentifer();

		RegisterMemberName(name);

		_stream.Consume(TokenType.Assign);

		Expression expression = new ExpressionParser(_assembly, _userClass, null, 
			ExpressionParsingMode.Statement, _stream).Parse();
		
		ConsumeTerminator();

		if (!expression.IsConstantEval())
		{
			throw new ParserException(head,
				$"User Class {_userClass} : Constant {name} had non-constant expression {expression}.");
		}

		Variant value;
		
		try
		{
			value = expression.Evaluate(null);
		}
		catch (Exception ex)
		{
			throw new RuntimeException(head, 
				$"User Class {_userClass} : Failed to initialize constant {name}.\n  --> {ex.Message}", ex);
		}

		Constant constant = new Constant(name, _userClass, value);
		
		_userClass.DeclaredConstants.Add(constant);
	}
	
	private void ParseConstructor()
	{
		if (_userClass.Constructor != null)
		{
			throw new ParserException(_stream,
				$"User Class {_userClass} : A constructor has already been defined.");
		}

		Token head = _stream.Read();
		
		(FunctionSignature signature, Variant[] defaultValues) = ParseFunctionSignature(false);

		ArraySegment<Token> baseCallTokens = GetBaseCallTokens();

		ArraySegment<Token> bodyTokens = GetFunctionBodyTokens();

		UserFunction function = new UserFunction(
			"new",
			_userClass,
			false,
			signature,
			defaultValues,
			head.Location,
			bodyTokens
		);

		_userClass.UserConstructor = new UserConstructor(_userClass, function)
		{
			BaseCallTokens = baseCallTokens,
		};
	}

	private ArraySegment<Token> GetStatementTokens(TokenType endToken)
	{
		int position = _stream.Position;

		_stream.SkipStatement(endToken);

		int count = _stream.Position - position;
        
		return _stream.Tokens.Slice(position, count);
	}
	
	private (FunctionSignature, Variant[]) ParseFunctionSignature(bool allowReturnType)
	{
		_stream.Consume(TokenType.OpenParen);

		var argumentNames = new HashSet<string>();

		var argumentList = new List<FunctionArgument>();

		var defaultValueList = new List<Variant>();

		if (!_stream.TryConsume(TokenType.CloseParen))
		{
			while (!_stream.EndOfStream)
			{
				Token nameToken = _stream.Consume(TokenType.Identifier);
				string name = nameToken.Value.AsString();

				if (!argumentNames.Add(name))
				{
					throw new ParserException(nameToken,
						$"User Class {_userClass} : Function argument '{name}' has already been defined.");
				}
			
				string pValueClass = _stream.TryConsume(TokenType.Colon) ? _stream.ConsumeIdentifer() : null;

				if (_stream.TryConsume(TokenType.Assign))
				{
					defaultValueList.Add(ParseDefaultValue());
				}
				else if (defaultValueList.Count != 0)
				{
					throw new ParserException(nameToken,
						$"User Class {_userClass} : Default arguments must all come at the end of a function.");
				}
			
				var argument = new FunctionArgument(name, pValueClass);
				
				argumentList.Add(argument);
				
				if (_stream.Peek().TokenType == TokenType.CloseParen)
				{
					break;
				}

				_stream.Consume(TokenType.Comma);
			}
			
			_stream.Consume(TokenType.CloseParen);
		}

		string pReturnType = allowReturnType && _stream.TryConsume(TokenType.Arrow)
			? _stream.ConsumeIdentifer()
			: null;

		FunctionArgument[] arguments = argumentList.ToArray();

		Variant[] defaultValues = defaultValueList.ToArray();
		
		var signature = new FunctionSignature(
			arguments,
			arguments.Length - defaultValues.Length,
			arguments.Length,
			pReturnType
		);
		
		return (signature, defaultValues);
	}

	private Variant ParseDefaultValue()
	{
		Token head = _stream.Peek();
		
		Expression expression =
			new ExpressionParser(_assembly, _userClass, null, ExpressionParsingMode.Argument, _stream).Parse();

		if (!expression.IsConstantEval())
		{
			throw new ParserException(head,
				"Default value expression must be constant evaluatable.");
		}

		try
		{
			return expression.Evaluate(null);
		}
		catch (Exception ex)
		{
			throw new ParserException(head,
				$"Failed to evaluate default value expression.\n --> {ex.Message}");
		}
	}
	
	private ArraySegment<Token> GetFunctionBodyTokens()
	{
		_stream.Consume(TokenType.OpenBrace);

		if (_stream.TryConsume(TokenType.CloseBrace))
		{
			return ArraySegment<Token>.Empty;
		}
        
		int position = _stream.Position;
        
		_stream.SkipBlock();
        
		int count = _stream.Position - position;

		_stream.Consume(TokenType.CloseBrace);
        
		return _stream.Tokens.Slice(position, count);
	}

	private ArraySegment<Token> GetBaseCallTokens()
	{
		if (!_stream.TryConsume(TokenType.Colon))
		{
			return ArraySegment<Token>.Empty;
		}

		_stream.Consume(TokenType.Base);

		return GetStatementTokens(TokenType.OpenBrace);
	}

	private void ParseEnum()
	{
		CreateClass();

		ParseEnumMembers();
	}

	private void ParseEnumMembers()
	{
		_stream.Consume(TokenType.OpenBrace);

		const string Names = "Names";
		const string Values = "Values";
		const string EnumSize = "EnumSize";
		
		var nameList = new List<string>();
		var valueList = new List<double>();

		if (!_stream.TryConsume(TokenType.CloseBrace))
		{
			double lastValue = -1;
		
			while (!_stream.EndOfStream)
			{
				Token nameToken = _stream.Consume(TokenType.Identifier);
				string name = nameToken.Value.AsString();

				if (name is Names or Values or EnumSize)
				{
					throw new ParserException(nameToken,
						$"Enum value name '{name}' is invalid as it is a reserved name.");
				}

				RegisterMemberName(name);
			
				double value;
			
				if (_stream.TryConsume(TokenType.Assign))
				{
					value = lastValue = GetEnumValue();
				}
				else
				{
					value = ++lastValue;
				}
				
				var constant = new Constant(name, _userClass, value);
			
				_userClass.DeclaredConstants.Add(constant);
				
				nameList.Add(name);
				valueList.Add(value);
			
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
		}

		var names = new PackedStringArray(nameList.ToArray());
		var values = new PackedNumberArray(valueList.ToArray());
		
		_userClass.CreateConstant(Names, names);
		_userClass.CreateConstant(Values, values);
		_userClass.CreateConstant(EnumSize, nameList.Count);
	}

	private double GetEnumValue()
	{
		Token head = _stream.Peek();
				
		Expression expression = new ExpressionParser(_assembly, _userClass, null,
			ExpressionParsingMode.Argument, _stream).Parse();

		if (!expression.IsConstantEval())
		{
			throw new ParserException(head,
				"Enum value had a non-constant expression.");
		}

		Variant result;
		
		try
		{
			result = expression.Evaluate(null);
		}
		catch (Exception ex)
		{
			throw new ParserException(head,
				$"Failed to evaluate enum expression.\n --> {ex.Message}");
		}

		if (result.VariantType != VariantType.Number)
		{
			throw new ParserException(head,
				$"Enum value must be a Number, got {result.Class}.");
		}

		return result.AsDouble();
	}
	
	private void ConsumeTerminator()
	{
		_stream.Consume(TokenType.Semicolon);
	}
}