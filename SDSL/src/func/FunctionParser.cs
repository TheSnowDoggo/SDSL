using SDSL.Expressions;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

public class FunctionParser
{
    private readonly TokenStream _stream;
    private readonly PrototypeFunction _prototypeFunction;

    private readonly PrototypeClass _containingClass;
    
    private readonly Dictionary<string, int> _variables = [];
    private readonly Stack<HashSet<string>> _scopes = [];
    private readonly Stack<int> _freeLocations = [];
    private int _locations;
    
    private readonly List<Statement> _statements = [];

    public FunctionParser(TokenStream stream, PrototypeFunction prototypeFunction)
    {
        _stream = stream;
        _prototypeFunction = prototypeFunction;
        _containingClass = prototypeFunction.Class;
    }
    
    public PrototypeFunction PrototypeFunction => _prototypeFunction;

    public Function Parse()
    {
        FunctionArg[] args = DefineArguments();
        
        SealClass returnType = _containingClass.ResolveDataTypeClass(_prototypeFunction.ReturnType);
        
        while (!_stream.EndOfStream)
        {
            Token head = _stream.Peek();

            switch (head.TokenType)
            {
            case TokenType.Var:
                ParseVariableDefinition(false);
                break;
            case TokenType.Const:
                ParseVariableDefinition(true);
                break;
            }
        }
        
        return new Function()
        {
            Name = _prototypeFunction.Name,
            Args = args,
            MinArgs = _prototypeFunction.ArgList.MinArgs,
            ReturnType = returnType,
            IsStatic = _prototypeFunction.IsStatic,
            Statements = _statements.ToArray(),
        };
    }

    public bool TryGetVariableLocation(string name, out int location)
    {
        return _variables.TryGetValue(name, out location);
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

    private void ParseVariableDefinition(bool isConst)
    {
        // Consume Var/Const
        Token head = _stream.Read();

        string name = _stream.ConsumeIdentifer();

        int location = DefineVariable(name);
        
        SealClass @class = ParseVariableClass();
        
        _statements.Add(new DefineStatement(location, @class, isConst));
    }

    private int DefineVariable(string name)
    {
        if (_variables.ContainsKey(name))
        {
            throw new LangException(_stream,
                $"Variable with name {name} has already been defined.");
        }
        
        if (!_scopes.TryPeek(out HashSet<string> variableNames))
        {
            throw new LangException(_stream,
                "No scopes have been defined.");
        }

        if (!variableNames.Add(name))
        {
            throw new LangException(_stream,
                $"Variable with name {name} already defined in this scope.");
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

            var stream = new TokenStream(prototypeArg.Tokens);
            var parser = new ExpressionParser(stream, ExpressionParsingMode.Statement, this);
            
            PackedExpression expression = parser.Parse();

            args[i] = new FunctionArg(
                prototypeArg.Name,
                @class,
                expression,
                prototypeArg.IsConst
            );
        }

        return args;
    }
}