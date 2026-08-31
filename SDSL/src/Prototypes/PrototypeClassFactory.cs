using System.Reflection;
using SDSL.Functions;

namespace SDSL.Prototypes;

// Generate prototype classes for native types
public static class PrototypeClassFactory
{
    public static void GenerateExportedClasses(
        PrototypeAssembly pAssembly,
        Assembly assembly)
    {
        Type[] types = assembly.GetExportedTypes();
        
        for (int i = 0; i < types.Length; i++)
        {
            Type type = types[i];
            
            var attribute = type.GetCustomAttribute<SealClassAttribute>();
            if (attribute == null)
                continue;

            SealClass sClass = GetCustomClass(type);

            PrototypeNamespace pNamespace = pAssembly.GetOrCreateNamespace(sClass.Namespace);
            
            GenerateClass(type, pNamespace, sClass);
        }
    }
    
    private static void GenerateClass(
        Type type,
        PrototypeNamespace pNamespace,
        SealClass sClass)
    {
        string name = sClass.Name;
        
        if (pNamespace.Classes.ContainsKey(name))
            throw new InvalidOperationException(
                $"Namespace {pNamespace} already contains class with name '{name}'.");

        var pClass = new PrototypeClass(pNamespace, sClass);

        MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
        for (int i = 0; i < methods.Length; i++)
            BindMethod(pClass, methods[i]);

        FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        for (int i = 0; i < fields.Length; i++)
            BindField(pClass, fields[i]);
        
        pNamespace.AddClass(pClass);
    }

    private static SealClass GetCustomClass(Type type)
    {
        SealClass exportedClass = null;
        
        foreach (FieldInfo fieldInfo in type.GetFields(
            BindingFlags.Static | BindingFlags.Public))
        {
            var attribute = fieldInfo.GetCustomAttribute<ClassExportAttribute>();
            if (attribute == null)
                continue;

            if (fieldInfo.GetValue(null) is not SealClass sClass)
                throw new InvalidOperationException(
                    $"Expected field {fieldInfo} to be assignable to type {typeof(SealClass)}.");

            if (exportedClass != null)
                throw new InvalidOperationException(
                    $"Type {type} cannot contain multiple CustomClassExports.");
            
            exportedClass = sClass;
        }
        
        if (exportedClass == null)
            throw new InvalidOperationException(
                $"Type {type} has no exported custom class and has not defined a namespace and name.");

        return exportedClass;
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

    private static PrototypeArgumentList ParseArgList(TokenStream stream)
    {
        stream.Consume(TokenType.OpenParen);

        if (stream.TryConsume(TokenType.CloseParen))
        {
            return PrototypeArgumentList.Empty;
        }

        var names = new HashSet<string>();
        var argList = new List<PrototypeArgument>();

        int optionalArgs = 0;
        bool isElipsed = false;

        while (!stream.EndOfStream)
        {
            Token identifierToken = stream.Consume(TokenType.Identifier);
            string name = identifierToken.Value.AsString();

            if (!names.Add(name))
            {
                throw new ParserException(identifierToken,
                    $"Function argument with name '{name}' has already been declared.");
            }

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
                {
                    throw new ParserException(stream,
                        "Argument list contained multiple elipse args.");
                }
                
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
                throw new ParserException(stream,
                    "All optional arguments must come at the end of the signature.");
            }
            
            argList.Add(new PrototypeArgument(
                name,
                dataType,
                false
            ));

            if (stream.Peek().TokenType == TokenType.CloseParen)
            {
                break;
            }

            if (isElipsed)
            {
                throw new ParserException(stream,
                    "Elipse argument must come at the end of the parameter list.");
            }

            stream.Consume(TokenType.Comma);
        }

        stream.Consume(TokenType.CloseParen);

        PrototypeArgument[] args = argList.ToArray();

        if (isElipsed)
        {
            int minArgs = args.Length - optionalArgs - 1;
            return new PrototypeArgumentList(args, minArgs, Function.AnyArgs);
        }
        else
        {
            int minArgs = args.Length - optionalArgs;
            return new PrototypeArgumentList(args, minArgs, args.Length);
        }
    }

    private static PrototypeFunction ParseSignature(
        PrototypeClass pClass,
        string signature,
        bool isStatic,
        Func<SealValue, SealValue[], SealValue> func)
    {
        Token[] tokens = new Tokenizer(signature).Tokenize();

        var stream = new TokenStream(tokens);
        
        string name = stream.TryConsume(TokenType.New)
            ? "new"
            : stream.ConsumeIdentifer();
        
        PrototypeArgumentList argList = ParseArgList(stream);

        PrototypeDataType returnType = stream.TryConsume(TokenType.Arrow)
            ? ParseDataType(stream)
            : PrototypeDataType.Any;

        if (!stream.EndOfStream)
        {
            throw new ParserException(stream,
                $"Uxexpected token {stream.Peek().TokenType}, signature is over!");
        }
        
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
        Func<SealValue, SealValue[], SealValue> func;
        
        switch (parameters.Length)
        {
        // Static function binding   
        case 1:
        {
            if (parameters[0].ParameterType != typeof(SealValue[]))
            {
                throw new InvalidOperationException(
                    $"Expected Method {methodInfo} parameter to be SealValue[], got {parameters[0].ParameterType}.");
            }

            isStatic = true;

            if (returnType == typeof(void))
            {
                var methodAction = methodInfo.CreateDelegate<Action<SealValue[]>>();

                func = (_, args) =>
                {
                    methodAction(args);
                    return SealValue.Nil;
                };
            }
            else
            {
                var methodFunc = methodInfo.CreateDelegate<Func<SealValue[], SealValue>>();
                
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
            
            if (parameters[1].ParameterType != typeof(SealValue[]))
                throw new InvalidOperationException(
                    $"Expected Method {methodInfo}'s second parameter to be SealValue[], got {parameters[1].ParameterType}.");

            isStatic = false;
            
            if (returnType == typeof(void))
            {
                var methodAction = methodInfo.CreateDelegate<Action<SealValue, SealValue[]>>();
                
                func = (self, args) =>
                {
                    methodAction(self, args);
                    return SealValue.Nil;
                };
            }
            else
            {
                func = methodInfo.CreateDelegate<Func<SealValue, SealValue[], SealValue>>();
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