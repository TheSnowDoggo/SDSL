using System.Reflection;

namespace SDSL.Prototypes;

// Generate prototype classes for native types
public class PrototypeClassFactory
{
    public static PrototypeClass Generate(
        Type type,
        PrototypeNamespace @namespace,
        string name,
        SealClass customClass)
    {
        if (@namespace.Classes.ContainsKey(name))
            throw new InvalidOperationException(
                $"Namespace {@namespace} already contains class with name '{name}'.");
        
        var @class = new PrototypeClass(@namespace, name, [], customClass);
        
        foreach (MethodInfo methodInfo in type.GetMethods(
            BindingFlags.Static | BindingFlags.Public))
        {
            BindMethod(@class, methodInfo);
        }
        
        @namespace.Classes.Add(name, @class);

        return @class;
    }

    private static PrototypeDataType ParseDataType(TokenStream stream)
    {
        string namespaceName = null;
        string className = stream.ConsumeIdentifer();

        if (stream.TryConsume(TokenType.Scope))
        {
            namespaceName = className;
            className = stream.ConsumeIdentifer();
        }
                
        return new PrototypeDataType(
            stream.Location,
            namespaceName,
            className
        );
    }

    private static PrototypeArgList ParseArgList(TokenStream stream)
    {
        stream.Consume(TokenType.OpenParen);
        
        if (stream.TryConsume(TokenType.CloseParen))
        {
            return PrototypeArgList.Empty;
        }

        var names = new HashSet<string>();
        var args = new List<PrototypeArg>();

        while (!stream.EndOfStream)
        {
            Token identifierToken = stream.Consume(TokenType.Identifier);
            string name = identifierToken.Value.AsString();

            if (!names.Add(name))
                throw new LangException(identifierToken,
                    $"Function argument with name {name} has already been declared.");

            PrototypeDataType dataType = stream.TryConsume(TokenType.Colon)
                ? ParseDataType(stream)
                : PrototypeDataType.Any;
            
            args.Add(new PrototypeArg(
                name,
                dataType,
                false
            ));
            
            if (stream.Peek().TokenType == TokenType.CloseParen)
                break;

            stream.Consume(TokenType.Comma);
        }

        stream.Consume(TokenType.CloseParen);
        
        return new PrototypeArgList(args.ToArray(), args.Count);
    }

    private static NativePrototypeFunction ParseSignature(
        PrototypeClass @class,
        string signature,
        bool isStatic,
        Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func)
    {
        Token[] tokens = new Tokenizer(signature).Tokenize();

        var stream = new TokenStream(tokens);

        string name = stream.ConsumeIdentifer();
        
        PrototypeArgList argList = ParseArgList(stream);

        PrototypeDataType returnType = stream.TryConsume(TokenType.Arrow)
            ? ParseDataType(stream)
            : PrototypeDataType.Any;

        if (!stream.EndOfStream)
            throw new LangException(stream,
                $"Uxexpected token {stream.Peek().TokenType}, signature is over!");
        
        return new NativePrototypeFunction(
            @class,
            name,
            argList,
            returnType,
            isStatic,
            func
        );
    }

    private static void BindMethod(PrototypeClass @class, MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<FunctionExportAttribute>();
        if (attribute == null)
            return;

        if (methodInfo.ReturnType != typeof(SealValue))
            throw new InvalidOperationException(
                $"Expected Method {methodInfo} to a return type of SealValue, got {methodInfo.ReturnType}.");

        ParameterInfo[] parameters = methodInfo.GetParameters();

        bool isStatic;
        Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func;
        
        switch (parameters.Length)
        {
        // Static function binding   
        case 1:
        {
            if (parameters[0].ParameterType != typeof(ReadOnlySpan<SealValue>))
                throw new InvalidOperationException(
                    $"Expected Method {methodInfo} parameter to be ReadOnlySpan<SealValue>, got {parameters[0].ParameterType}.");

            isStatic = true;
            
            var methodDelegate = methodInfo.CreateDelegate<Func<ReadOnlySpan<SealValue>, SealValue>>();
            func = (_, args) => methodDelegate(args);
            
            break;
        }
        // Member function binding
        case 2:
        {
            if (parameters[0].ParameterType != typeof(SealValue))
                throw new InvalidOperationException(
                    $"Expected Method {methodInfo}'s first parameter to be SealValue, got {parameters[0].ParameterType}.");
            
            if (parameters[1].ParameterType != typeof(ReadOnlySpan<SealValue>))
                throw new InvalidOperationException(
                    $"Expected Method {methodInfo}'s second parameter to be ReadOnlySpan<SealValue>, got {parameters[1].ParameterType}.");

            isStatic = false;
            
            func = methodInfo.CreateDelegate<Func<SealValue, ReadOnlySpan<SealValue>, SealValue>>();
            
            break;
        }
        default:
            throw new InvalidOperationException(
                $"Expected Method {methodInfo} to have 1 or 2 parameters, got {parameters.Length}.");
        }

        NativePrototypeFunction prototypeFunction = ParseSignature(
            @class,
            attribute.Signature,
            isStatic,
            func
        );
        
        if (!@class.Functions.TryAdd(prototypeFunction.Name, prototypeFunction))
            throw new InvalidOperationException(
                $"Class {@class} already contains a function with name {prototypeFunction.Name}.");
    }
}