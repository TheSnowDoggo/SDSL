namespace SDSL.Prototypes;

public class PrototypeParser
{
    private readonly TokenStream _stream;
    private readonly PrototypeAssembly _assembly;

    private PrototypeNamespace _namespace;
    private PrototypeClass _class;
    
    private string[] _usings;
    private bool _noTerminators;
    
    public PrototypeParser(TokenStream stream, PrototypeAssembly assembly)
    {
        _stream = stream;
        _assembly = assembly;
    }

    public void Parse()
    {
        ParseFlag();
        
        ParseUsings();

        while (!_stream.EndOfStream)
        {
            ParseNext();
        }
    }
    
    private void ConsumeTerminator()
    {
        if (!_noTerminators)
        {
            _stream.Consume(TokenType.Semicolon);
        }
    }

    private void ParseFlag()
    {
        if (!_stream.TryConsume(TokenType.Not))
            return;

        string flag = _stream.ConsumeIdentifer();

        switch (flag)
        {
        case "no_terminators":
            _noTerminators = true;
            break;
        default:
            throw new ParserException(_stream,
                $"Read unknown flag: '{flag}'.");
        }
    }

    private void ParseUsings()
    {
        var usings = new HashSet<string>();
        
        while (_stream.TryPeek(out Token head)
               && head.TokenType == TokenType.Using)
        {
            _stream.Read();
            
            if (!_stream.TryConsume(TokenType.Identifier, out Token identifierToken))
            {
                throw new ParserException(identifierToken,
                    $"Expected a namespace identifier following using keyword, got {identifierToken.TokenType}.");
            }

            string identifier = identifierToken.Value.AsString();

            if (!usings.Add(identifier))
            {
                throw new ParserException(identifierToken,
                    $"Using namespace with name {identifier} was already declared.");
            }

            ConsumeTerminator();
        }
        
        // Add global usings
        usings.UnionWith(_assembly.GlobalUsings);
        
        _usings = usings.ToArray();
    }

    private void ParseNext()
    {
        // Implicit global namespace
        if (_stream.Peek().TokenType == TokenType.Class)
        {
            _namespace = _assembly.GetOrCreateNamespace(GlobalConfig.GlobalNamespace);
            ParseClass();
            return;
        }
        
        _stream.Consume(TokenType.Namespace);

        string name = _stream.ConsumeIdentifer();

        _namespace = _assembly.GetOrCreateNamespace(name);

        // Scoped namespace
        if (_stream.TryConsume(TokenType.OpenBrace))
        {
            while (!_stream.TryConsume(TokenType.CloseBrace))
                ParseClass();
            return;
        }

        ConsumeTerminator();
        
        // File level namespace
        while (!_stream.EndOfStream)
            ParseClass();
    }
    
    private void ParseClass()
    {
        _stream.Consume(TokenType.Class);

        Token identifierToken = _stream.Consume(TokenType.Identifier);
        string name = identifierToken.Value.AsString();

        var sClass = new SealClass(
            _namespace.Name,
            name,
            ValueType.Object,
            false
        );
        
        _class = new PrototypeClass(
            _namespace,
            sClass
        ) {
            UsingsNames = _usings,
            NoTerminators = _noTerminators,
        };
        
        if (!_namespace.Classes.TryAdd(name, _class))
        {
            throw new ParserException(identifierToken,
                $"Class with name '{name}' has already been declared in namespace '{_namespace.Name}'.");
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
                    throw new ParserException(token, "Static modifier is not valid for a constructor.");
                }
                
                ParseConstructor();
                
                break;
            case TokenType.Constepxr:
                if (isStatic)
                {
                    throw new ParserException(token, "Static modifier is not valid for a constexpr member.");
                }

                ParseConstant();
                
                break;
            default:
                throw new ParserException(token, $"Unexpected token type {token.TokenType} in class defintion.");
            }
        }
    }
    
    private string GetCurrentClassName()
    {
        return $"{_namespace.Name}::{_class.Name}";
    }

    private void CheckForDuplicateMemberName(SourceLocation error, string memberName)
    {
        if (_class.HasMember(memberName))
        {
            throw new ParserException(error,
                $"Member with name '{memberName}' has already been declared in class {GetCurrentClassName()}.");
        }
    }
    
    private PrototypeDataType GetParsedDataType()
    {
        Token identifierToken = _stream.Consume(TokenType.Identifier);
        string name = identifierToken.Value.AsString();
     
        string pNamespace = null;

        if (_stream.TryConsume(TokenType.Scope))
        {
            pNamespace = name;
            name = _stream.ConsumeIdentifer();
        }

        return new PrototypeDataType(
            identifierToken.Location,
            pNamespace,
            name
        );
    }

    private PrototypeDataType GetParsedDataTypeAnnotation()
    {
        return _stream.TryConsume(TokenType.Colon) ? GetParsedDataType() : PrototypeDataType.Any;
    }

    private ArraySegment<Token> GetParsedAssignmentExpression(bool isStatement)
    {
        if (!_stream.TryConsume(TokenType.Assign))
        {
            return ArraySegment<Token>.Empty;
        }
        
        int position = _stream.Position;

        if (isStatement)
        {
            _stream.SkipStatement(_noTerminators);
        }
        else
        {
            _stream.SkipArgument();
        }

        int count = _stream.Position - position;
        
        return _stream.Tokens.Slice(position, count);
    }
    
    private void ParseField(bool isStatic, bool isConst)
    {
        Token head = _stream.Read();
        
        string name = _stream.ConsumeIdentifer();
        
        CheckForDuplicateMemberName(head.Location, name);

        PrototypeDataType dataType = GetParsedDataTypeAnnotation();

        ArraySegment<Token> tokens = GetParsedAssignmentExpression(isStatement: true);
        
        ConsumeTerminator();

        var protoField = new PrototypeField(
            _class,
            name,
            dataType,
            tokens,
            isConst,
            isStatic
        );
        
        _class.Fields.Add(name, protoField);
    }

    private PrototypeArgumentList GetParsedArgList()
    {
        _stream.Consume(TokenType.OpenParen);
        
        if (_stream.TryConsume(TokenType.CloseParen))
        {
            return PrototypeArgumentList.Empty;
        }

        var names = new HashSet<string>();
        var argList = new List<PrototypeArgument>();

        int defaultArgs = 0;

        while (!_stream.EndOfStream)
        {
            bool isConst = _stream.TryConsume(TokenType.Const);
        
            Token identifierToken = _stream.Consume(TokenType.Identifier);
            string name = identifierToken.Value.AsString();

            if (!names.Add(name))
            {
                throw new ParserException(identifierToken,
                    $"Function argument with name '{name}' has already been declared.");
            }

            PrototypeDataType dataType = GetParsedDataTypeAnnotation();

            ArraySegment<Token> expression = GetParsedAssignmentExpression(isStatement: false);

            if (expression.Count != 0)
            {
                defaultArgs++;
            }
            else if (defaultArgs != 0)
            {
                throw new ParserException(identifierToken,
                    $"Function argument with name '{name}' must have a default expression.");
            }

            var arg = new PrototypeArgument(
                name,
                dataType,
                isConst,
                expression
            );
            
            argList.Add(arg);

            if (_stream.TryConsume(TokenType.CloseParen))
                break;

            _stream.Consume(TokenType.Comma);
        }
        
        PrototypeArgument[] args = argList.ToArray();

        int minArgs = args.Length - defaultArgs;

        return new PrototypeArgumentList(args, minArgs, args.Length);
    }
    
    private ArraySegment<Token> GetParsedFunctionBody()
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

    private void ParseFunction(bool isStatic)
    {
        Token head = _stream.Read();
        
        string name = _stream.ConsumeIdentifer();

        CheckForDuplicateMemberName(head.Location, name);
            
        PrototypeArgumentList argList = GetParsedArgList();

        var returnType = PrototypeDataType.Any;
        
        if (_stream.TryConsume(TokenType.Arrow))
        {
            returnType = GetParsedDataType();   
        }

        ArraySegment<Token> tokens = GetParsedFunctionBody();

        var protoFunc = new PrototypeFunction(
            head.Location,
            _class,
            name,
            argList,
            returnType,
            isStatic,
            new UserFunctionBody(tokens)
        );
        
        _class.Functions.Add(name, protoFunc);
    }

    private void ParseConstructor()
    {
        Token head = _stream.Read();
        
        if (_class.Constructor != null)
        {
            throw new ParserException(head,
                $"Class {GetCurrentClassName()} cannot contain multiple constructors.");
        }
        
        PrototypeArgumentList argList = GetParsedArgList();
        ArraySegment<Token> tokens = GetParsedFunctionBody();

        var pFunction = new PrototypeFunction(
            head.Location,
            _class,
            "new",
            argList,
            PrototypeDataType.Any,
            false,
            new UserFunctionBody(tokens)
        );

        _class.Constructor = pFunction;
    }

    private void ParseConstant()
    {
        Token head = _stream.Read();
        
        string name = _stream.ConsumeIdentifer();
        
        CheckForDuplicateMemberName(head.Location, name);
        
        ArraySegment<Token> tokens = GetParsedAssignmentExpression(isStatement: true);

        ConsumeTerminator();

        var pConstant = new PrototypeConstant(
            _class,
            name,
            tokens
        );
        
        _class.Constants.Add(name, pConstant);
    }
}