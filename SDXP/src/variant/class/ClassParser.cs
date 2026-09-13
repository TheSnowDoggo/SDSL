namespace SDSL;

public class ClassParser
{
	private readonly VariantAssembly _assembly;
	private readonly TokenStream _stream;

	private UserVariantClass _class;
	private HashSet<string> _memberNames;
	
	public ClassParser(VariantAssembly assembly, TokenStream stream)
	{
		_assembly = assembly;
		_stream = stream;
	}

	public void Parse()
	{
		while (!_stream.EndOfStream)
		{
			ParseClass();
		}
	}

	private void ParseClass()
	{
		_stream.Consume(TokenType.Class);

		Token nameToken = _stream.Consume(TokenType.Identifier);
		string className = nameToken.Value.AsString();
		
		_class = new UserVariantClass(className);
		_memberNames = [];

		if (!_assembly.Classes.TryAdd(className, _class))
		{
			throw new ParserException(nameToken,
				$"Class with name {className} has already been defined.");
		}

		_assembly.UserClasses.Add(_class);

		ParseBaseClass();

		ParseClassMembers();
	}

	private void ParseBaseClass()
	{
		if (!_stream.TryConsume(TokenType.Colon))
		{
			return;
		}

		string identifier = _stream.ConsumeIdentifer();

		_class.PrototypeBaseClass = identifier;
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
						$"User Class {_class} : Static qualifier is disallowed for constant definition.");
				}
				
				ParseConstant();
				break;
			// Constructor definition
			case TokenType.New:
				if (isStatic)
				{
					throw new ParserException(_stream,
						$"User Class {_class} : Static qualifier is disallowed for constructor definition.");
				}
				
				ParseConstructor();
				break;
			default:
				throw new ParserException(head,
					$"User Class {_class} : Got unexpected token {head} in class definition.");
			}
		}

		_stream.Consume(TokenType.CloseBrace);
	}

	private void ParseField(bool isStatic)
	{
		// Consume var
		Token head = _stream.Read();
		
		string name = _stream.ConsumeIdentifer();

		RegisterMemberName(name);

		string pValueClass;
		ArraySegment<Token> tokens;

		if (_stream.TryConsume(TokenType.TypeAssign))
		{
			pValueClass = ImplicitVariantClass.ImplicitName;
			
			tokens = GetAssignmentTokens(isStatement: true);
		}
		else
		{
			pValueClass = _stream.TryConsume(TokenType.Colon)
				? _stream.ConsumeIdentifer()
				: null;

			tokens = _stream.TryConsume(TokenType.Assign)
				? GetAssignmentTokens(isStatement: true)
				: ArraySegment<Token>.Empty;
		}

		ConsumeTerminator();

		Property property;
		
		if (isStatic)
		{
			property = new UserStaticProperty(name, _class, pValueClass, head.Location, tokens);
		}
		else
		{
			property = new UserInstanceProperty(name, _class, pValueClass, head.Location, tokens);
		}
		
		_class.DeclaredProperties.Add(property);
	}
	
	private void ParseFunction(bool isStatic)
	{
		Token head = _stream.Read();
		
		string name = _stream.ConsumeIdentifer();

		RegisterMemberName(name);

		FunctionSignature signature = ParseFunctionSignature();

		ArraySegment<Token> tokens = GetFunctionBodyTokens();

		UserFunction function = new UserFunction(
			name,
			_class,
			isStatic,
			signature,
			head.Location,
			tokens
		);
		
		_class.DeclaredFunctions.Add(function);
	}

	private void ParseConstant()
	{
		throw new NotImplementedException();
		
		_stream.Advance();
		
		string name = _stream.ConsumeIdentifer();

		RegisterMemberName(name);

		_stream.Consume(TokenType.Assign);

		GetAssignmentTokens(isStatement: true);
		
		ConsumeTerminator();
	}
	
	private void ParseConstructor()
	{
		if (_class.Constructor != null)
		{
			throw new ParserException(_stream,
				$"User Class {_class} : A constructor has already been defined.");
		}

		Token head = _stream.Read();
		
		FunctionSignature signature = ParseFunctionSignature();

		ArraySegment<Token> tokens = GetFunctionBodyTokens();

		UserFunction function = new UserFunction(
			"new",
			_class,
			true,
			signature,
			head.Location,
			tokens
		);

		_class.Constructor = function;
	}

	private void RegisterMemberName(string name)
	{
		if (!_memberNames.Add(name))
		{
			throw new ParserException(_stream,
				$"User Class {_class} : A duplicate member with name '{name}' has already been defined.");
		}
	}
	
	private ArraySegment<Token> GetAssignmentTokens(bool isStatement)
	{
		int position = _stream.Position;

		if (isStatement)
		{
			_stream.SkipStatement();
		}
		else
		{
			_stream.SkipArgument();
		}

		int count = _stream.Position - position;
        
		return _stream.Tokens.Slice(position, count);
	}
	
	private FunctionSignature ParseFunctionSignature()
	{
		_stream.Consume(TokenType.OpenParen);

		var argumentNames = new HashSet<string>();

		var argumentList = new List<FunctionArgument>();

		if (!_stream.TryConsume(TokenType.CloseParen))
		{
			while (!_stream.EndOfStream)
			{
				Token nameToken = _stream.Consume(TokenType.Identifier);
				string name = nameToken.Value.AsString();

				if (!argumentNames.Add(name))
				{
					throw new ParserException(nameToken,
						$"User Class {_class} : Function argument '{name}' has already been defined.");
				}
			
				string pValueClass = _stream.TryConsume(TokenType.Colon)
					? _stream.ConsumeIdentifer()
					: null;
			
				FunctionArgument argument = new FunctionArgument(
					name,
					pValueClass
				);
				
				argumentList.Add(argument);
				
				if (_stream.Peek().TokenType == TokenType.CloseParen)
				{
					break;
				}

				_stream.Consume(TokenType.Comma);
			}
			
			_stream.Consume(TokenType.CloseParen);
		}

		string pReturnType = _stream.TryConsume(TokenType.Arrow)
			? _stream.ConsumeIdentifer()
			: null;

		FunctionArgument[] arguments = argumentList.ToArray();

		return new FunctionSignature(
			arguments,
			arguments.Length,
			arguments.Length,
			pReturnType
		);
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

	private void ConsumeTerminator()
	{
		_stream.Consume(TokenType.Semicolon);
	}
}