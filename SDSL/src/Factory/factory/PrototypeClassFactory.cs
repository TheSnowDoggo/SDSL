using System.Reflection;
using SDSL.Functions;
using SDSL.Prototypes;

namespace SDSL.Factory;

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
            
            var nativeAttribute = type.GetCustomAttribute<NativeClassAttribute>();

            if (nativeAttribute != null)
            {
                SealClass sClass = GetClassExport(type);

                PrototypeNamespace pNamespace = pAssembly.GetOrCreateNamespace(sClass.Namespace);
            
                GenerateClass(type, pNamespace, sClass);
            }

            var customAttribute = type.GetCustomAttribute<CustomClassGeneratorAttribute>();

            if (customAttribute != null)
            {
                GenerateCustom(type, pAssembly, customAttribute);
            }
        }
    }

    private static void GenerateCustom(Type type, PrototypeAssembly pAssembly, CustomClassGeneratorAttribute customClassAttribute)
    {
        string methodName = customClassAttribute.GenerateMethodName ?? "Generate";

        MethodInfo methodInfo = type.GetMethod(methodName);

        if (methodInfo == null)
        {
            throw new NativeFactoryException(
                $"{type} Custom generator method with name '{methodName}' not found.");
        }

        if (!methodInfo.IsStatic)
        {
            throw new NativeFactoryException(
                $"{type} Custom generator method must be static.");
        }

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();

        if (parameterInfos.Length != 1)
        {
            throw new NativeFactoryException(
                $"{type} Custom generator method take one parameter, got {parameterInfos.Length}.");
        }
                
        if (parameterInfos[0].ParameterType != typeof(PrototypeAssembly))
        {
            throw new NativeFactoryException(
                $"{type} Custom generator method parameter must be type {typeof(PrototypeAssembly)}, got {parameterInfos[0].ParameterType}.");
        }
                
        if (methodInfo.ReturnType != typeof(void))
        {
            throw new NativeFactoryException(
                $"{type} Custom generator method must return type {typeof(void)}, got {methodInfo.ReturnType}.");
        }

        methodInfo.Invoke(null, [pAssembly]);
    }
    
    private static SealClass GetClassExport(Type type)
    {
        SealClass exportedClass = null;

        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfos[i];
            
            var attribute = fieldInfo.GetCustomAttribute<ClassExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            if (fieldInfo.GetValue(null) is not SealClass sClass)
            {
                throw new NativeFactoryException(
                    $"Expected field {fieldInfo} to be assignable to type {typeof(SealClass)}.");
            }

            if (exportedClass != null)
            {
                throw new NativeFactoryException(
                    $"Type {type} cannot contain multiple CustomClassExports.");
            }
            
            exportedClass = sClass;
        }

        if (exportedClass == null)
        {
            throw new NativeFactoryException(
                $"Type {type} has no exported custom class and has not defined a namespace and name.");
        }

        return exportedClass;
    }
    
    private static void GenerateClass(
        Type type,
        PrototypeNamespace pNamespace,
        SealClass sClass)
    {
        string name = sClass.Name;

        if (pNamespace.Classes.ContainsKey(name))
        {
            throw new NativeFactoryException(
                $"Namespace {pNamespace} already contains class with name '{name}'.");
        }

        var pClass = new PrototypeClass(pNamespace, sClass);

        var memberNames = new HashSet<string>();

        BindMethods(type, pClass, memberNames);

        BindConstants(type, pClass, memberNames);
        
        pNamespace.AddClass(pClass);
    }
    
    private static void BindMethods(Type type, PrototypeClass pClass, HashSet<string> memberNames)
    {
        MethodInfo[] typeMethods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
        
        for (int i = 0; i < typeMethods.Length; i++)
        {
            MethodInfo methodInfo = typeMethods[i];
            
            var attribute = methodInfo.GetCustomAttribute<FunctionExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            Type returnType = methodInfo.ReturnType;
            
            if (returnType != typeof(SealValue)
                && returnType != typeof(void))
            {
                throw new NativeFactoryException(
                    $"Expected Method {methodInfo} to a return type of SealValue or void, got {methodInfo.ReturnType}.");
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();

            bool isStatic;
            NativeDelegate func;
            
            switch (parameters.Length)
            {
            // Static function binding   
            case 1:
            {
                if (parameters[0].ParameterType != typeof(SealValue[]))
                {
                    throw new NativeFactoryException(
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
                {
                    throw new NativeFactoryException(
                        $"Expected Method {methodInfo}'s first parameter to be SealValue, got {parameters[0].ParameterType}.");
                }

                if (parameters[1].ParameterType != typeof(SealValue[]))
                {
                    throw new NativeFactoryException(
                        $"Expected Method {methodInfo}'s second parameter to be SealValue[], got {parameters[1].ParameterType}.");
                }

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
                    func = methodInfo.CreateDelegate<NativeDelegate>();
                }
                
                break;
            }
            default:
                throw new NativeFactoryException(
                    $"Expected Method {methodInfo} to have 1 or 2 parameters, got {parameters.Length}.");
            }

            PrototypeFunction pFunction = ParseSignature(
                pClass,
                attribute.Signature,
                isStatic,
                func
            );
            
            if (!memberNames.Add(pFunction.Name))
            {
                throw new NativeFactoryException(
                    $"Class {pClass} already contains a function with name {pFunction.Name}.");
            }

            // Not a constructor
            if (pFunction.Name != "new")
            {
                pClass.NativeFunctions.Add(pFunction);
                
                continue;
            }

            if (!isStatic)
            {
                throw new NativeFactoryException(
                    $"{methodInfo} was invalid: Constructor must be static.");
            }
            
            pClass.Constructor = pFunction;
        }
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
        NativeDelegate func)
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
            SourceLocation.Native,
            pClass,
            name,
            argList,
            returnType,
            isStatic,
            new NativeFunctionBody(func)
        );
    }
    
    private static void BindConstants(Type type, PrototypeClass pClass, HashSet<string> memberNames)
    {
        FieldInfo[] typeFields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);

        for (int i = 0; i < typeFields.Length; i++)
        {
            FieldInfo fieldInfo = typeFields[i];
            
            var attribute = fieldInfo.GetCustomAttribute<ConstantExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            string name = attribute.Name ?? fieldInfo.Name;
            
            if (!memberNames.Add(name))
            {
                throw new NativeFactoryException(
                    $"Class {pClass} already contains a field with name {name}.");
            }
        
            object obj = fieldInfo.GetValue(null);
        
            SealValue value = SealValue.FromObject(obj);

            var pConstant = new PrototypeConstant(
                SourceLocation.Native,
                name,
                value
            );
            
            pClass.NativeConstants.Add(pConstant);
        }
    }
}