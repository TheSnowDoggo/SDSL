namespace SDSL.Prototypes;

public class PrototypeParser
{
    private readonly TokenStream _stream;
    private readonly PrototypeAssembly _assembly;

    private string[] _usings;

    private PrototypeNamespace _namespace;
    private PrototypeClass _class;
    
    public PrototypeParser(TokenStream stream, PrototypeAssembly assembly)
    {
        _stream = stream;
        _assembly = assembly;
    }

    public void Parse()
    {
        ParseUsings();

        while (!_stream.EndOfStream)
        {
            ParseNamespace();
        }
    }

    private void ParseUsings()
    {
        var usings = new HashSet<string>();
        
        while (_stream.TryPeek(out Token head))
        {
            if (head.TokenType == TokenType.Namespace)
            {
                break;
            }

            if (head.TokenType != TokenType.Using)
            {
                throw new LangException(head,
                    $"Expected using or namespace declaration, got {head.TokenType}.");
            }
            
            _stream.Read();
            
            if (!_stream.TryConsume(TokenType.Identifier, out Token identifierToken))
            {
                throw new LangException(identifierToken,
                    $"Expected a namespace identifier following using keyword, got {identifierToken.TokenType}.");
            }

            string identifier = identifierToken.Value.AsString();

            if (!usings.Add(identifier))
            {
                throw new LangException(identifierToken,
                    $"Using namespace with name {identifier} was already declared.");
            }

            _stream.Consume(TokenType.Semicolon);
        }
        
        _usings = usings.ToArray();
    }

    private void ParseNamespace()
    {
        _stream.Consume(TokenType.Namespace);

        string name = _stream.ConsumeIdentifer();

        _namespace = _assembly.GetOrCreateNamespace(name);
        
        _stream.Consume(TokenType.OpenBrace);

        while (!_stream.TryConsume(TokenType.CloseBrace))
        {
            ParseClass();
        }
    }

    private void ParseClass()
    {
        _stream.Consume(TokenType.Class);

        Token identifierToken = _stream.Consume(TokenType.Identifier);
        string name = identifierToken.Value.AsString();

        _class = new PrototypeClass(
            _namespace,
            name,
            _usings
        );
        
        if (!_namespace.Classes.TryAdd(name, _class))
        {
            throw new LangException(identifierToken,
                $"Class with name {name} has already been declared in namespace {_namespace.Name}.");
        }

        _stream.Consume(TokenType.OpenBrace);

        while (!_stream.TryConsume(TokenType.CloseBrace, out Token token))
        {
            bool isStatic = false;

            if (token.TokenType == TokenType.Static)
            {
                _stream.Advance();
                token = _stream.Peek();
                isStatic = true;
            }

            switch (token.TokenType)
            {
            case TokenType.Var:
                ParseField(isStatic, false);
                break;
            case TokenType.Const:
                ParseField(isStatic, true);
                break;
            case TokenType.Func:
                ParseFunction(isStatic);
                break;
            case TokenType.New:
                if (isStatic)
                {
                    throw new LangException(token, "Static constructors do not exist.");
                }
                ParseConstructor();
                break;
            default:
                throw new LangException(token, $"Unexpected token type {token.TokenType} in class defintion.");
            }
        }
        
        _class.GenerateClass();
    }
    
    private string GetCurrentClassName()
    {
        return $"{_namespace.Name}::{_class.Name}";
    }

    private void CheckForDuplicateMemberName(SourceLocation error, string memberName)
    {
        if (_class.Functions.ContainsKey(memberName)
            || _class.Fields.ContainsKey(memberName))
        {
            throw new LangException(error,
                $"Function or Field with name {memberName} has already been declared in class {GetCurrentClassName()}.");
        }
    }
    
    private PrototypeDataType GetParsedDataType()
    {
        Token identifierToken = _stream.Consume(TokenType.Identifier);
        string name = identifierToken.Value.AsString();
     
        string @namespace = null;

        if (_stream.TryConsume(TokenType.Scope))
        {
            @namespace = name;
            name = _stream.ConsumeIdentifer();
        }

        return new PrototypeDataType(
            identifierToken.Location,
            @namespace,
            name
        );
    }

    private ArraySegment<Token> GetParsedAssignmentExpression(Predicate<TokenType> endCondition)
    {
        if (!_stream.TryConsume(TokenType.Assign))
        {
            return ArraySegment<Token>.Empty;
        }
        
        int position = _stream.Position;

        while (_stream.TryPeek(out Token token)
               && !endCondition(token.TokenType))
        {
            _stream.Advance();
        }
        
        int count = _stream.Position - position;
        
        return _stream.Tokens.Slice(position, count);
    }

    private void ParseField(bool isStatic, bool isConst)
    {
        Token head = _stream.Read();
        
        string name = _stream.ConsumeIdentifer();
        
        CheckForDuplicateMemberName(head.Location, name);

        _stream.Consume(TokenType.Colon);

        PrototypeDataType dataType = GetParsedDataType();

        ArraySegment<Token> expression = GetParsedAssignmentExpression(
            static tokenType => tokenType is TokenType.Semicolon);

        _stream.Consume(TokenType.Semicolon);

        var protoField = new PrototypeField(
            _class,
            name,
            dataType,
            expression,
            isConst,
            isStatic
        );
        
        _class.Fields.Add(name, protoField);
    }

    private PrototypeArgList GetParsedArgList()
    {
        _stream.Consume(TokenType.OpenParen);
        
        if (_stream.TryConsume(TokenType.CloseParen))
        {
            return PrototypeArgList.Empty;
        }

        var names = new HashSet<string>();
        var args = new List<PrototypeArg>();

        int defaultArgs = 0;

        while (!_stream.EndOfStream)
        {
            bool isConst = _stream.TryConsume(TokenType.Const);
        
            Token identifierToken = _stream.Consume(TokenType.Identifier);
            string name = identifierToken.Value.AsString();

            if (!names.Add(name))
            {
                throw new LangException(identifierToken,
                    $"Function argument with name {name} has already been declared.");
            }

            var dataType = PrototypeDataType.Any;

            if (_stream.TryConsume(TokenType.Colon))
            {
                dataType = GetParsedDataType();
            }

            ArraySegment<Token> expression = GetParsedAssignmentExpression(
                static tokenType => tokenType is TokenType.Comma or TokenType.CloseParen);

            if (expression.Count != 0)
            {
                defaultArgs++;
            }
            else if (defaultArgs != 0)
            {
                throw new LangException(identifierToken,
                    $"Function argument with name {name} must have a default expression.");
            }

            var arg = new PrototypeArg(
                name,
                dataType,
                expression,
                isConst
            );
            
            args.Add(arg);

            if (_stream.TryConsume(TokenType.CloseParen))
            {
                break;
            }

            _stream.Consume(TokenType.Comma);
        }

        int minArgs = args.Count - defaultArgs;

        return new PrototypeArgList(args.ToArray(), minArgs);
    }
    
    private ArraySegment<Token> GetParsedFunctionBody()
    {
        _stream.Consume(TokenType.OpenBrace);

        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            return ArraySegment<Token>.Empty;
        }
        
        int position = _stream.Position;
        int bracketDepth = 0;

        while (_stream.TryPeek(out Token token))
        {
            if (token.TokenType == TokenType.OpenBrace)
            {
                _stream.Advance();
                bracketDepth++;
                
                continue;
            }

            if (token.TokenType == TokenType.CloseBrace)
            {
                if (bracketDepth == 0)
                {
                    break;
                }
                
                _stream.Advance();
                bracketDepth--;
                
                continue;
            }
            
            _stream.Advance();
        }
        
        int count = _stream.Position - position;

        _stream.Consume(TokenType.CloseBrace);
        
        return _stream.Tokens.Slice(position, count);
    }

    private void ParseFunction(bool isStatic)
    {
        Token head = _stream.Read();
        
        string name = _stream.ConsumeIdentifer();

        CheckForDuplicateMemberName(head.Location, name);
            
        PrototypeArgList argList = GetParsedArgList();

        var returnType = PrototypeDataType.Any;
        
        if (_stream.TryConsume(TokenType.Arrow))
        {
            returnType = GetParsedDataType();   
        }

        ArraySegment<Token> tokens = GetParsedFunctionBody();

        var protoFunc = new PrototypeFunction(
            _class,
            name,
            argList,
            returnType,
            tokens,
            isStatic
        );
        
        _class.Functions.Add(name, protoFunc);
    }

    private void ParseConstructor()
    {
        Token head = _stream.Read();
        
        if (_class.Constructor != null)
        {
            throw new LangException(head,
                $"Class {GetCurrentClassName()} cannot contain multiple constructors.");
        }
        
        PrototypeArgList argList = GetParsedArgList();
        ArraySegment<Token> tokens = GetParsedFunctionBody();

        var protoConstructor = new PrototypeConstructor(
            argList,
            tokens
        );

        _class.Constructor = protoConstructor;
    }
}