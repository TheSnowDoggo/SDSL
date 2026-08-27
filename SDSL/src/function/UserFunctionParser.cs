using SDSL.Expressions;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

public class UserFunctionParser
{
    private readonly SealAssembly _assembly;
    private readonly UserPrototypeFunction _prototypeFunction;
    private readonly PrototypeClass _containingClass;
    
    private readonly TokenStream _stream;
    
    private readonly Dictionary<string, int> _variables = [];
    private readonly Stack<HashSet<string>> _scopes = [];
    private readonly Stack<int> _freeLocations = [];
    private int _locations;
    
    public UserFunctionParser(SealAssembly assembly, UserPrototypeFunction prototypeFunction)
    {
        _assembly = assembly;
        _prototypeFunction = prototypeFunction;
        _containingClass = prototypeFunction.Class;

        _stream = new TokenStream(_prototypeFunction.Tokens);
    }
    
    public UserPrototypeFunction PrototypeFunction => _prototypeFunction;

    public UserFunction Parse()
    {
        OpenScope();

        if (!_prototypeFunction.IsStatic)
        {
            DefineVariable(Function.SelfName);
        }
        
        FunctionArg[] args = DefineArguments();
        
        SealClass returnType = _containingClass.ResolveDataTypeClass(_prototypeFunction.ReturnType);

        var statements = new List<Statement>();
        
        while (!_stream.EndOfStream)
        {
            statements.Add(ParseStatement());
        }
        
        return new UserFunction(statements.ToArray(), _locations)
        {
            Assembly = _assembly,
            Class = _containingClass.Class,
            Name = _prototypeFunction.Name,
            Args = args,
            MinArgs = _prototypeFunction.ArgList.MinArgs,
            ReturnType = returnType,
            IsStatic = _prototypeFunction.IsStatic,
        };
    }

    public bool TryGetVariableLocation(string name, out int location)
    {
        return _variables.TryGetValue(name, out location);
    }

    public int GetVariableLocation(string name)
    {
        return _variables[name];
    }
    
    private ExpressionParser CreateExpressionParser(ExpressionParsingMode parsingMode)
    {
        return new ExpressionParser(_stream, parsingMode, this);
    }

    private void OpenScope()
    {
        _scopes.Push([]);
    }

    private void CloseScope()
    {
        if (!_scopes.TryPop(out HashSet<string> variableNames))
        {
            throw new LangException(_stream,
                "No scopes have been defined.");
        }

        foreach (string name in variableNames)
        {
            if (!_variables.Remove(name, out int location))
            {
                throw new LangException(_stream,
                    $"Failed to delete variable {name}.");
            }
            
            _freeLocations.Push(location);
        }
    }

    private SealClass ParseVariableClass()
    {
        if (!_stream.TryConsume(TokenType.Colon))
        {
            return null;
        }
        
        string className = _stream.ConsumeIdentifer();

        if (_stream.TryConsume(TokenType.Scope))
        {
            string namespaceName = className;
            className = _stream.ConsumeIdentifer();
            
            return _containingClass.ResolveFullClass(_stream.Location, namespaceName, className).Class;
        }
        else
        {
            return _containingClass.ResolveImplicitClass(_stream.Location, className).Class;
        }
    }

    private int DefineVariable(string name)
    {
        if (_variables.ContainsKey(name))
        {
            throw new LangException(_stream,
                $"Variable '{name}' has already been defined.");
        }
        
        if (!_scopes.TryPeek(out HashSet<string> variableNames))
        {
            throw new LangException(_stream,
                "No scopes have been defined.");
        }

        if (!variableNames.Add(name))
        {
            throw new LangException(_stream,
                $"Variable '{name}' already defined in this scope.");
        }

        if (!_freeLocations.TryPop(out int location))
        {
            location = _locations++;
        }
        
        _variables.Add(name, location);

        return location;
    }

    private FunctionArg[] DefineArguments()
    {
        PrototypeArg[] prototypeArgs = _prototypeFunction.ArgList.Args;
        int length = prototypeArgs.Length;
        
        var args = new FunctionArg[length];
        
        for (int i = 0; i < length; i++)
        {
            PrototypeArg prototypeArg = prototypeArgs[i];
            
            DefineVariable(prototypeArg.Name);
            
            SealClass @class = _containingClass.ResolveDataTypeClass(prototypeArg.DataType);

            Expression expression = null;
            
            if (prototypeArg.Tokens.Count != 0)
            {
                var stream = new TokenStream(prototypeArg.Tokens);
                
                var parser = new ExpressionParser(
                    stream,
                    ExpressionParsingMode.Statement,
                    this
                );
            
                expression = parser.Parse();
            }

            args[i] = new FunctionArg(
                prototypeArg.Name,
                @class,
                expression,
                prototypeArg.IsConst
            );
        }

        return args;
    }
    
    private Statement[] ParseStatements()
    {
        _stream.Consume(TokenType.OpenBrace);

        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            return [];
        }
        
        OpenScope();
        
        var statements = new List<Statement>();

        while (!_stream.EndOfStream)
        {
            statements.Add(ParseStatement());
            
            if (_stream.Peek().TokenType == TokenType.CloseBrace)
                break;
        }

        _stream.Consume(TokenType.CloseBrace);
        
        CloseScope();
        
        return statements.ToArray();
    }
    
    private Statement ParseStatement()
    {
        Token head = _stream.Peek();

        return head.TokenType switch
        {
            TokenType.Var
                => ParseDefinitionStatement(false),
            TokenType.Const
                => ParseDefinitionStatement(true),
            TokenType.Identifier
                => ParseExpressionStatement(),
            TokenType.Return
                => ParseReturnStatement(),
            TokenType.OpenBrace
                => ParseBlockStatement(),
            _ => throw new LangException(head.Location, $"Unknown statement starting token: {head.TokenType}.")
        };
    }
    
    private DefineStatement ParseDefinitionStatement(bool isConst)
    {
        // Consume Var/Const
        Token head = _stream.Read();

        string name = _stream.ConsumeIdentifer();
        
        SealClass @class = ParseVariableClass();
        
        Expression expression = null;
        
        if (_stream.TryConsume(TokenType.Assign))
        {
            expression = CreateExpressionParser(ExpressionParsingMode.Statement).Parse();
        }

        _stream.Consume(TokenType.Semicolon);
        
        // Make sure to define variable after parsing the assignment expression
        // otherwise the variable could reference itself
        int refLocation = DefineVariable(name);

        return new DefineStatement(
            head.Location,
            refLocation,
            @class,
            isConst,
            expression
        );
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        // Do not consume starting identifer, but we need location
        Token head = _stream.Peek();
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Statement).Parse();
        
        _stream.Consume(TokenType.Semicolon);

        return new ExpressionStatement(
            head.Location,
            expression
        );
    }

    private ReturnStatement ParseReturnStatement()
    {
        // Consume return
        Token head = _stream.Read();
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Statement).Parse();

        _stream.Consume(TokenType.Semicolon);

        return new ReturnStatement(
            head.Location,
            expression
        );
    }

    private BlockStatement ParseBlockStatement()
    {
        Token head = _stream.Peek();
        
        Statement[] statements = ParseStatements();
        
        return new BlockStatement(
            head.Location,
            statements
        );
    }

    private IfStatement ParseIfStatement()
    {
        // Consume if
        Token head = _stream.Read();

        Expression condition = CreateExpressionParser(ExpressionParsingMode.Condition)
            .Parse();

        Statement[] statements = ParseStatements();

        throw new NotImplementedException();
    }
}