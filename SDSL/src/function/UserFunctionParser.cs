using SDSL.Expressions;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

public class UserFunctionParser
{
    private readonly PrototypeFunction _pFunction;
    private readonly PrototypeClass _containingClass;
    
    private readonly TokenStream _stream;
    
    private readonly Dictionary<string, int> _variables = [];
    private readonly Stack<HashSet<string>> _scopes = [];
    private readonly Stack<int> _freeLocations = [];
    
    private int _locations;
    
    public UserFunctionParser(
        TokenStream stream,
        PrototypeFunction pFunction)
    {
        _pFunction = pFunction;
        _containingClass = pFunction.Class;

        _stream = stream;
    }
    
    public PrototypeFunction PrototypeFunction => _pFunction;

    public UserFunction Parse()
    {
        OpenScope();

        if (!_pFunction.IsStatic)
        {
            DefineVariable(Function.SelfName);
        }
        
        FunctionArgument[] args = DefineArguments();
        
        SealClass returnType = _containingClass.ResolveDataTypeClass(_pFunction.ReturnType);

        var statements = new List<Statement>();
        
        while (!_stream.EndOfStream)
        {
            statements.Add(ParseStatement());
        }
        
        return new UserFunction(
            _pFunction.Location,
            statements.ToArray(),
            _locations
        ) {
            Class = _containingClass.Class,
            Name = _pFunction.Name,
            Args = args,
            MinArgs = _pFunction.ArgList.MinArgs,
            MaxArgs = _pFunction.ArgList.MaxArgs,
            ReturnType = returnType,
            IsStatic = _pFunction.IsStatic
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

        if (className == "Any")
            return null;
        
        return _containingClass.ResolveImplicitClass(_stream.Location, className).Class;
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

    private FunctionArgument[] DefineArguments()
    {
        PrototypeArgument[] prototypeArgs = _pFunction.ArgList.Args;
        int length = prototypeArgs.Length;
        
        var args = new FunctionArgument[length];
        
        for (int i = 0; i < length; i++)
        {
            PrototypeArgument prototypeArgument = prototypeArgs[i];
            
            DefineVariable(prototypeArgument.Name);
            
            SealClass pClass = _containingClass.ResolveDataTypeClass(prototypeArgument.DataType);

            Expression expression = null;
            
            if (prototypeArgument.Tokens.Count != 0)
            {
                var stream = new TokenStream(prototypeArgument.Tokens);
                
                var parser = new ExpressionParser(
                    stream,
                    ExpressionParsingMode.Statement,
                    this
                );
            
                expression = parser.Parse(false);
            }

            args[i] = new FunctionArgument(
                prototypeArgument.Name,
                pClass,
                expression,
                prototypeArgument.IsConst
            );
        }

        return args;
    }
    
    private Statement[] ParseStatements(bool openScope = true)
    {
        _stream.Consume(TokenType.OpenBrace);

        if (_stream.TryConsume(TokenType.CloseBrace))
            return [];
        
        if (openScope)
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

    private void SkipEmptyStatements()
    {
        if (_containingClass.NoTerminators)
            return;
        
        while (_stream.TryPeek(out Token token)
               && token.TokenType is TokenType.Semicolon)
        {
            _stream.Advance();
        }
    }
    
    private Statement ParseStatement()
    {
        SkipEmptyStatements();
        
        Token head = _stream.Peek();

        return head.TokenType switch
        {
            TokenType.Var
                => ParseDefinitionStatement(false),
            TokenType.Const
                => ParseDefinitionStatement(true),
            TokenType.Identifier
                or TokenType.New
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
            _ => throw new LangException(head.Location, $"Unknown statement starting token: {head.TokenType}.")
        };
    }
    
    private DefineStatement ParseDefinitionStatement(bool isConst)
    {
        // Consume Var/Const
        Token head = _stream.Read();

        string name = _stream.ConsumeIdentifer();
        
        SealClass pClass = ParseVariableClass();
        
        Expression expression = null;
        
        if (_stream.TryConsume(TokenType.Assign))
        {
            expression = CreateExpressionParser(ExpressionParsingMode.Statement)
                .Parse(false);
        }

        ConsumeTerminator();
        
        // Make sure to define variable after parsing the assignment expression
        // otherwise the variable could reference itself
        int refLocation = DefineVariable(name);

        return new DefineStatement(
            head.Location,
            refLocation,
            pClass,
            isConst,
            expression
        );
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        // Do not consume starting identifer, but we need location
        Token head = _stream.Peek();
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Statement)
            .Parse(false);
        
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
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Statement)
            .Parse(true);

        ConsumeTerminator();

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
            .Parse(false);

        Statement[] statements = ParseStatements();

        BlockStatement elseBlock = _stream.TryConsume(TokenType.Else)
            ? _stream.Peek().TokenType == TokenType.If
                ? ParseIfStatement()
                : ParseBlockStatement()
            : null;

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
        
        Expression condition = CreateExpressionParser(ExpressionParsingMode.Condition)
            .Parse(false);
        
        Statement[] statements = ParseStatements();

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
        OpenScope();
        
        SealClass pClass = ParseVariableClass();
        
        int variableLocation = DefineVariable(identifier);

        _stream.Consume(TokenType.In);
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Condition)
            .Parse(false);

        Statement[] statements = ParseStatements(openScope: false);

        return new ForStatement(
            head.Location,
            statements,
            variableLocation,
            pClass,
            expression
        );
    }

    private void ConsumeTerminator()
    {
        if (!_containingClass.NoTerminators)
            _stream.Consume(TokenType.Semicolon);
    }
}