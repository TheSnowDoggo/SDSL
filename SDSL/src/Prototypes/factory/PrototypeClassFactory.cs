using System.Reflection;

namespace SDSL.Prototypes;

// Generate prototype classes for native types
public static class PrototypeClassFactory
{
    public static void GenerateClass(
        Type type,
        PrototypeNamespace pNamespace,
        SealClass sClass)
    {
        string name = sClass.Name;
        
        if (pNamespace.Classes.ContainsKey(name))
            throw new InvalidOperationException(
                $"Namespace {pNamespace} already contains class with name '{name}'.");

        var pClass = new PrototypeClass(pNamespace, sClass);
        
        foreach (MethodInfo methodInfo in type.GetMethods(
            BindingFlags.Static | BindingFlags.Public))
        {
            BindMethod(pClass, methodInfo);
        }

        foreach (FieldInfo fieldInfo in type.GetFields(
            BindingFlags.Static | BindingFlags.Public))
        {
            BindField(pClass, fieldInfo);
        }
        
        pNamespace.AddClass(pClass);
    }

    public static void GenerateExportedClasses(
        PrototypeAssembly pAssembly,
        Assembly assembly)
    {
        foreach (Type type in assembly.GetExportedTypes())
        {
            var attribute = type.GetCustomAttribute<ClassExportAttribute>();
            if (attribute == null)
                continue;

            SealClass sClass;
            
            if (attribute.Namespace == null || attribute.Name == null)
            {
                sClass = GetCustomClass(type);

                if (sClass == null)
                    throw new InvalidOperationException(
                        $"Type {type} has no exported custom class and has not defined a namespace and name.");
            }
            else
            {
                sClass = new SealClass(
                    attribute.Namespace,
                    attribute.Name,
                    ValueType.Object,
                    true
                );
            }

            PrototypeNamespace pNamespace = pAssembly.GetOrCreateNamespace(sClass.Namespace);
            
            GenerateClass(type, pNamespace, sClass);
        }
    }

    private static SealClass GetCustomClass(Type type)
    {
        SealClass customClass = null;
        
        foreach (FieldInfo fieldInfo in type.GetFields(
            BindingFlags.Static | BindingFlags.Public))
        {
            var attribute = fieldInfo.GetCustomAttribute<CustomClassExportAttribute>();
            if (attribute == null)
                continue;

            if (fieldInfo.GetValue(null) is not SealClass sealClass)
                throw new InvalidOperationException(
                    $"Expected field {fieldInfo} to be assignable to type {typeof(SealClass)}.");

            if (customClass != null)
                throw new InvalidOperationException(
                    $"Type {type} cannot contain multiple CustomClassExports.");
            
            customClass = sealClass;
        }

        return customClass;
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
            return PrototypeArgList.Empty;

        var names = new HashSet<string>();
        var argList = new List<PrototypeArgument>();

        int optionalArgs = 0;
        bool isElipsed = false;

        while (!stream.EndOfStream)
        {
            Token identifierToken = stream.Consume(TokenType.Identifier);
            string name = identifierToken.Value.AsString();

            if (!names.Add(name))
                throw new LangException(identifierToken,
                    $"Function argument with name '{name}' has already been declared.");

            var dataType = PrototypeDataType.Any;

            switch (stream.Peek().TokenType)
            {
            case TokenType.Colon:
                stream.Advance();
                dataType = ParseDataType(stream);
                
                break;
            case TokenType.Elipse:
                stream.Advance();
                
                if (isElipsed)
                    throw new LangException(stream,
                        "Argument list contained multiple elipse args.");
                
                isElipsed = true;
                
                break;
            }

            if (stream.TryConsume(TokenType.Assign))
            {
                stream.Consume(TokenType.Question);
                optionalArgs++;
            }
            else if (optionalArgs != 0)
            {
                throw new LangException(stream,
                    "All optional arguments must come at the end of the signature.");
            }
            
            argList.Add(new PrototypeArgument(
                name,
                dataType,
                false
            ));
            
            if (stream.Peek().TokenType == TokenType.CloseParen)
                break;

            if (isElipsed)
                throw new LangException(stream,
                    "Elipse argument must come at the end of the parameter list.");

            stream.Consume(TokenType.Comma);
        }

        stream.Consume(TokenType.CloseParen);

        PrototypeArgument[] args = argList.ToArray();

        if (isElipsed)
        {
            int minArgs = args.Length - optionalArgs - 1;
            return new PrototypeArgList(args, minArgs, Function.AnyArgs);
        }
        else
        {
            int minArgs = args.Length - optionalArgs;
            return new PrototypeArgList(args, minArgs, args.Length);
        }
    }

    private static PrototypeFunction ParseSignature(
        PrototypeClass pClass,
        string signature,
        bool isStatic,
        Func<SealValue, ReadOnlySpan<SealValue>, SealValue> func)
    {
        Token[] tokens = new Tokenizer(signature).Tokenize();

        var stream = new TokenStream(tokens);
        
        string name = stream.TryConsume(TokenType.New)
            ? "new"
            : stream.ConsumeIdentifer();
        
        PrototypeArgList argList = ParseArgList(stream);

        PrototypeDataType returnType = stream.TryConsume(TokenType.Arrow)
            ? ParseDataType(stream)
            : PrototypeDataType.Any;

        if (!stream.EndOfStream)
            throw new LangException(stream,
                $"Uxexpected token {stream.Peek().TokenType}, signature is over!");
        
        return new PrototypeFunction(
            SourceLocation.Invalid,
            pClass,
            name,
            argList,
            returnType,
            isStatic,
            new NativeFunctionBody(func)
        );
    }

    private static void BindMethod(PrototypeClass pClass, MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<FunctionExportAttribute>();
        if (attribute == null)
            return;

        Type returnType = methodInfo.ReturnType;
        
        if (returnType != typeof(SealValue)
            && returnType != typeof(void))
        {
            throw new InvalidOperationException(
                $"Expected Method {methodInfo} to a return type of SealValue or void, got {methodInfo.ReturnType}.");
        }

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

            if (returnType == typeof(void))
            {
                var methodAction = methodInfo.CreateDelegate<Action<ReadOnlySpan<SealValue>>>();

                func = (_, args) =>
                {
                    methodAction(args);
                    return SealValue.Nil;
                };
            }
            else
            {
                var methodFunc = methodInfo.CreateDelegate<Func<ReadOnlySpan<SealValue>, SealValue>>();
                
                func = (_, args) => methodFunc(args);
            }
            
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
            
            if (returnType == typeof(void))
            {
                var methodAction = methodInfo.CreateDelegate<Action<SealValue, ReadOnlySpan<SealValue>>>();
                
                func = (self, args) =>
                {
                    methodAction(self, args);
                    return SealValue.Nil;
                };
            }
            else
            {
                func = methodInfo.CreateDelegate<Func<SealValue, ReadOnlySpan<SealValue>, SealValue>>();
            }
            
            break;
        }
        default:
            throw new InvalidOperationException(
                $"Expected Method {methodInfo} to have 1 or 2 parameters, got {parameters.Length}.");
        }

        PrototypeFunction prototypeFunction = ParseSignature(
            pClass,
            attribute.Signature,
            isStatic,
            func
        );

        // Not a constructor
        if (prototypeFunction.Name != "new")
        {
            if (!pClass.Functions.TryAdd(prototypeFunction.Name, prototypeFunction))
                throw new InvalidOperationException(
                    $"Class {pClass} already contains a function with name {prototypeFunction.Name}.");
            return;
        }

        if (pClass.Constructor != null)
            throw new InvalidOperationException(
                $"{methodInfo} was invalid: Class {pClass} has already defined a constructor {pClass.Constructor.Name}.");

        if (!isStatic)
            throw new InvalidOperationException(
                $"{methodInfo} was invalid: Constructor must be static.");
        
        pClass.Constructor = prototypeFunction;
    }

    private static void BindField(PrototypeClass pClass, FieldInfo fieldInfo)
    {
        var attribute = fieldInfo.GetCustomAttribute<ConstantExportAttribute>();
        if (attribute == null)
            return;

        string name = attribute.Name ?? fieldInfo.Name;
        
        object obj = fieldInfo.GetValue(null);
        
        SealValue value = SealValue.FromObject(obj);

        var pField = new PrototypeField(
            pClass,
            name,
            new PrototypeDataType(
                SourceLocation.Invalid,
                value.Class.Namespace,
                value.Class.Name
            ),
            new Token[] { new Token(
                SourceLocation.Invalid,
                TokenType.Literal,
                value
            ) },
            true,
            true
        );

        if (!pClass.Fields.TryAdd(name, pField))
            throw new InvalidOperationException(
                $"Class {pClass} already contains a field with name {name}.");
    }
}