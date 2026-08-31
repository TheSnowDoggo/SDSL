using SDSL.Expressions;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL.Functions;

public class UserFunctionParser
{
    private readonly TokenStream _stream;
    
    private readonly PrototypeFunction _pFunction;
    private readonly PrototypeClass _containingClass;
    
    // Maps all the variables currently defined to their stack location
    private readonly Dictionary<string, int> _variableMap = [];
    // A stack containing the sets of the variable names defined in the current scope
    private readonly Stack<List<string>> _scopes = [];
    // All the locations which are currently not in use
    private readonly Stack<int> _freeLocations = [];
    // The const-ness of all currently defined variables
    private readonly List<VariableDefinition> _variables = [];
    
    public UserFunctionParser(
        TokenStream stream,
        PrototypeFunction pFunction)
    {
        _stream = stream;
        _pFunction = pFunction;
        _containingClass = pFunction.Class;
    }
    
    public PrototypeFunction PrototypeFunction => _pFunction;

    public UserFunction Parse()
    {
        OpenScope();

        if (!_pFunction.IsStatic)
        {
            DefineVariable(Function.SelfName, true);
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
            _containingClass.Class,
            _pFunction.Name,
            args,
            _pFunction.ArgList.MinArgs,
            _pFunction.ArgList.MaxArgs,
            returnType,
            _pFunction.IsStatic,
            statements.ToArray(),
            _variables.Count
        );
    }

    public bool TryGetVariableLocation(string name, out int location)
    {
        return _variableMap.TryGetValue(name, out location);
    }

    public int GetVariableLocation(string name)
    {
        return _variableMap[name];
    }

    public VariableDefinition GetVariableDefinition(int location)
    {
        return _variables[location];
    }

    private void OpenScope()
    {
        _scopes.Push([]);
    }

    private void CloseScope()
    {
        if (!_scopes.TryPop(out List<string> variableNames))
        {
            throw new ParserException(_stream,
                "Tried to close scope but no scopes have been defined.");
        }

        for (int i = 0; i < variableNames.Count; i++)
        {
            string name = variableNames[i];
            
            if (!_variableMap.Remove(name, out int location))
            {
                throw new ParserException(_stream,
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
        {
            return null;
        }
        
        return _containingClass.ResolveImplicitClass(_stream.Location, className).Class;
    }

    private int DefineVariable(string name, bool isConst)
    {
        if (_variableMap.ContainsKey(name))
        {
            throw new ParserException(_stream,
                $"Variable '{name}' has already been defined.");
        }
        
        if (!_scopes.TryPeek(out List<string> variableNames))
        {
            throw new ParserException(_stream,
                "No scopes have been defined.");
        }
        
        variableNames.Add(name);

        var definition = new VariableDefinition(name, isConst);

        if (_freeLocations.TryPop(out int location))
        {
            _variables[location] = definition;
        }
        else
        {
            location = _variables.Count;
            _variables.Add(definition);
        }

        _variableMap.Add(name, location);

        return location;
    }

    private FunctionArgument[] DefineArguments()
    {
        PrototypeArgument[] prototypeArgs = _pFunction.ArgList.Args;
        int length = prototypeArgs.Length;
        
        var args = new FunctionArgument[length];
        
        for (int i = 0; i < length; i++)
        {
            PrototypeArgument pArg = prototypeArgs[i];
            
            DefineVariable(pArg.Name, pArg.IsConst);
            
            SealClass pClass = _containingClass.ResolveDataTypeClass(pArg.DataType);

            Expression expression = null;
            
            if (pArg.Tokens.Count != 0)
            {
                var stream = new TokenStream(pArg.Tokens);
                
                var parser = new ExpressionParser(
                    stream,
                    this,
                    ExpressionParsingMode.Statement
                );
            
                expression = parser.Parse();
            }

            args[i] = new FunctionArgument(
                pArg.Name,
                pClass,
                expression
            );
        }

        return args;
    }
    
    private void SkipEmptyStatements()
    {
        if (_containingClass.NoTerminators)
        {
            return;
        }
        
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
            TokenType.Identifier or TokenType.New
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
            _ => throw new ParserException(head.Location, $"Got unexpected token {head.TokenType} parsing statement."),
        };
    }
    
    private Statement[] ParseStatements(bool openScope = true)
    {
        _stream.Consume(TokenType.OpenBrace);

        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            return [];
        }

        if (openScope)
        {
            OpenScope();
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
        
        CloseScope();
        
        return statements.ToArray();
    }
    
    private void ConsumeTerminator()
    {
        if (!_containingClass.NoTerminators)
        {
            _stream.Consume(TokenType.Semicolon);
        }
    }
    
    private ExpressionParser CreateExpressionParser(ExpressionParsingMode parsingMode)
    {
        return new ExpressionParser(_stream, this, parsingMode);
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
            expression = CreateExpressionParser(ExpressionParsingMode.Statement).Parse();
        }

        ConsumeTerminator();
        
        // Make sure to define variable after parsing the assignment expression
        // otherwise the variable could reference itself
        int refLocation = DefineVariable(name, isConst);

        return new DefineStatement(
            head.Location,
            refLocation,
            pClass,
            expression
        );
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        // Do not consume starting identifer, but we need location
        Token head = _stream.Peek();
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Statement).Parse();
        
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
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Statement).Parse(true);

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

        Expression condition = CreateExpressionParser(ExpressionParsingMode.Condition).Parse();

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
        
        Expression condition = CreateExpressionParser(ExpressionParsingMode.Condition).Parse();
        
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
        
        int variableLocation = DefineVariable(identifier, true);

        _stream.Consume(TokenType.In);
        
        Expression expression = CreateExpressionParser(ExpressionParsingMode.Condition)
            .Parse();

        Statement[] statements = ParseStatements(openScope: false);

        return new ForStatement(
            head.Location,
            statements,
            variableLocation,
            pClass,
            expression
        );
    }
}