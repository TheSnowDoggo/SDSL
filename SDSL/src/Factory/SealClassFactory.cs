using System.Reflection;
using SDSL.Functions;
using SDSL.Prototypes;

namespace SDSL.Factory;

public static class SealClassFactory
{
	public const string DefaultGenerateMethodName = "Generate";
	
	[Flags]
	public enum FunctionFlags
	{
		None = 0,
		Args = 1,
		Void = 2,
		Self = 4,
	}

	public static void GenerateExportedClasses(
		PrototypeAssembly pAssembly,
		Assembly assembly)
	{
		Type[] types = assembly.GetExportedTypes();

		for (int i = 0; i < types.Length; i++)
		{
			Type type = types[i];

			ClassExportAttribute attribute = type.GetCustomAttribute<ClassExportAttribute>();

			if (attribute == null)
			{
				continue;
			}
			
			InvokeGenerator(type, attribute, pAssembly);
		}
	}

	public static void GenerateNativeClasses(PrototypeAssembly pAssembly)
	{
		GenerateExportedClasses(pAssembly, Assembly.GetAssembly(typeof(SealClassFactory)));
	}
	
	public static void Generate(
		Type type,
		PrototypeAssembly pAssembly,
		SealClass sClass,
		Func<MethodInfo, FunctionFlags, NativeDelegate> instanceMethodBinder = null)
	{
		PrototypeClass pClass = pAssembly.CreateClass(sClass);
		
		var memberNames = new HashSet<string>();
		
		BindMethods(type, pClass, memberNames, instanceMethodBinder);
		
		BindConstants(type, pClass, memberNames);
	}

	public static void Generate<TObject>(PrototypeAssembly pAssembly, SealClass sClass)
		where TObject : SealObject
	{
		Generate(typeof(TObject), pAssembly, sClass, BindInstanceMethod<TObject>);
	}
	
	public static bool HasFunctionFlag(this FunctionFlags source, FunctionFlags flag)
	{
		return (source & flag) == flag;
	}

	private static void InvokeGenerator(Type type,
		ClassExportAttribute attribute,
		PrototypeAssembly pAssembly)
	{
		string methodName = attribute.GenerateMethodName ?? DefaultGenerateMethodName;

		MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

		if (methodInfo == null)
		{
			throw new NativeFactoryException($"No public static method called {methodName} found in type {type}.");
		}

		ParameterInfo[] parameterInfos = methodInfo.GetParameters();

		if (parameterInfos.Length != 1)
		{
			throw new NativeFactoryException($"Generate method {methodInfo} in {type} must take one argument.");
		}
		
		if (parameterInfos[0].ParameterType != typeof(PrototypeAssembly))
		{
			throw new NativeFactoryException(
				$"Generate method {methodInfo} in {type} must have parameter of type {typeof(PrototypeAssembly)}, got {parameterInfos[0].ParameterType}.");
		}

		methodInfo.Invoke(null, [pAssembly]);
	}
	
	private static void BindMethods(
		Type type,
		PrototypeClass pClass,
		HashSet<string> memberNames,
		Func<MethodInfo, FunctionFlags, NativeDelegate> instanceMethodBinder = null)
	{
		MethodInfo[] methodInfos = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

		for (int i = 0; i < methodInfos.Length; i++)
		{
			MethodInfo methodInfo = methodInfos[i];

			FunctionExportAttribute exportAttribute = methodInfo.GetCustomAttribute<FunctionExportAttribute>();

			if (exportAttribute == null)
			{
				continue;
			}
			
			string name = exportAttribute.Name ?? methodInfo.Name;
			
			if (!memberNames.Add(name))
			{
				throw new NativeFactoryException(
					$"Class {pClass} already contains a member with name '{name}'.");
			}
			
			FunctionFlags flags = GetFunctionFlags(methodInfo);

			bool isStatic = methodInfo.IsStatic;

			NativeDelegate func;

			if (isStatic)
			{
				if (flags.HasFlag(FunctionFlags.Self))
				{
					func = BindStaticSelfMethod(methodInfo, flags);
					isStatic = false;
				}
				else
				{
					func = BindStaticMethod(methodInfo, flags);
				}
			}
			else
			{
				if (instanceMethodBinder == null)
				{
					throw new NativeFactoryException(
						$"Got instance method {methodInfo} but no instance binder was assigned.");
				}

				func = instanceMethodBinder(methodInfo, flags);
			}

			FunctionInfoAttribute infoAttribute = methodInfo.GetCustomAttribute<FunctionInfoAttribute>();

			PrototypeArgumentList args = CreateArgumentList(exportAttribute, infoAttribute);
			
			var pFunction = new PrototypeFunction(
				SourceLocation.Invalid,
				pClass,
				name,
				args,
				PrototypeDataType.Any,
				isStatic,
				PrototypeType.Native,
				func
			);

			if (methodInfo.GetCustomAttribute<SealConstructorAttribute>() == null)
			{
				pClass.NativeFunctions.Add(pFunction);
				continue;
			}

			if (pClass.Constructor != null)
			{
				throw new NativeFactoryException(
					$"Class {pClass} cannot conatin multiple constructors.");
			}

			pClass.Constructor = pFunction;
		}
	}
	
	private static void BindConstants(Type type, PrototypeClass pClass, HashSet<string> memberNames)
	{
		FieldInfo[] fieldInfos = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);

		for (int i = 0; i < fieldInfos.Length; i++)
		{
			FieldInfo fieldInfo = fieldInfos[i];
            
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

			var pConstant = new PrototypeConstant(SourceLocation.Native, name, value);
            
			pClass.NativeConstants.Add(pConstant);
		}
	}

	private static FunctionFlags GetParameterFlags(MethodInfo methodInfo)
	{
		ParameterInfo[] parameterInfos = methodInfo.GetParameters();
		
		switch (parameterInfos.Length)
		{
		case 0:
			return FunctionFlags.None;
		case 1:
		{
			Type firstType = parameterInfos[0].ParameterType;

			if (firstType == typeof(SealValue))
			{
				if (!methodInfo.IsStatic)
				{
					throw new NativeFactoryException("Native function with self parameter must be static.");
				}
				
				return FunctionFlags.Self;
			}
			
			if (firstType == typeof(SealValue[]))
			{
				return FunctionFlags.Args;
			}
			
			throw new NativeFactoryException(
				$"Native function first parameter must be of type {typeof(SealValue[])} or {typeof(SealValue)}, got {firstType}.");
		}
		case 2:
			if (!methodInfo.IsStatic)
			{
				throw new NativeFactoryException("Native function with self parameter must be static.");
			}
			
			if (parameterInfos[0].ParameterType != typeof(SealValue))
			{
				throw new NativeFactoryException($"Native function self paramater must be of type {typeof(SealValue)}, got {parameterInfos[0].ParameterType}.");
			}
			
			if (parameterInfos[1].ParameterType != typeof(SealValue[]))
			{
				throw new NativeFactoryException($"Native function args paramater must be of type {typeof(SealValue[])}, got {parameterInfos[1].ParameterType}.");
			}

			return FunctionFlags.Args | FunctionFlags.Self;
		default:
			throw new NativeFactoryException($"Native function must take 0 or 1 parameter(s), got {parameterInfos.Length}.");
		}
	}
	
	private static FunctionFlags GetReturnFlags(Type returnType)
	{
		if (returnType == typeof(void))
		{
			return FunctionFlags.Void;
		}
		
		if (returnType == typeof(SealValue))
		{
			return FunctionFlags.None;
		}

		throw new NativeFactoryException($"Native function must return {typeof(SealValue)} or {typeof(void)}, got {returnType}.");
	}
	
	private static FunctionFlags GetFunctionFlags(MethodInfo methodInfo)
	{
		FunctionFlags flags = FunctionFlags.None;

		flags |= GetParameterFlags(methodInfo);
		flags |= GetReturnFlags(methodInfo.ReturnType);

		return flags;
	}
	
	private static NativeDelegate BindStaticMethod(MethodInfo methodInfo, FunctionFlags functionFlags)
	{
		if (functionFlags.HasFunctionFlag(FunctionFlags.Void))
		{
			if (functionFlags.HasFunctionFlag(FunctionFlags.Args))
			{
				var action = methodInfo.CreateDelegate<Action<SealValue[]>>();
				return (_, args) => { action(args); return SealValue.Nil; };
			}
			else
			{
				var action = methodInfo.CreateDelegate<Action>();
				return (_, _) => { action(); return SealValue.Nil; };
			}
		}
		else
		{
			if (functionFlags.HasFunctionFlag(FunctionFlags.Args))
			{
				var func = methodInfo.CreateDelegate<Func<SealValue[], SealValue>>();
				return (_, args) => func(args);
			}
			else
			{
				var func = methodInfo.CreateDelegate<Func<SealValue>>();
				return (_, _) => func();
			}
		}
	}

	private static NativeDelegate BindStaticSelfMethod(MethodInfo methodInfo, FunctionFlags functionFlags)
	{
		if (functionFlags.HasFunctionFlag(FunctionFlags.Void))
		{
			if (functionFlags.HasFunctionFlag(FunctionFlags.Args))
			{
				var action = methodInfo.CreateDelegate<Action<SealValue, SealValue[]>>();
				return (self, args) => { action(self, args); return SealValue.Nil; };
			}
			else
			{
				var action = methodInfo.CreateDelegate<Action<SealValue>>();
				return (self, _) => { action(self); return SealValue.Nil; };
			}
		}
		else
		{
			if (functionFlags.HasFunctionFlag(FunctionFlags.Args))
			{
				var func = methodInfo.CreateDelegate<Func<SealValue, SealValue[], SealValue>>();
				return (self, args) => func(self, args);
			}
			else
			{
				var func = methodInfo.CreateDelegate<Func<SealValue, SealValue>>();
				return (self, _) => func(self);
			}
		}
	}
	
	private static NativeDelegate BindInstanceMethod<TObject>(MethodInfo methodInfo, FunctionFlags functionFlags)
		where TObject : SealObject
	{
		if (functionFlags.HasFunctionFlag(FunctionFlags.Void))
		{
			if (functionFlags.HasFunctionFlag(FunctionFlags.Args))
			{
				var action = methodInfo.CreateDelegate<Action<TObject, SealValue[]>>();
				return (self, args) => { action(self.AsSealObject<TObject>(), args); return SealValue.Nil; };
			}
			else
			{
				var action = methodInfo.CreateDelegate<Action<TObject>>();
				return (self, _) => { action(self.AsSealObject<TObject>()); return SealValue.Nil; };
			}
		}
		else
		{
			if (functionFlags.HasFunctionFlag(FunctionFlags.Args))
			{
				var func = methodInfo.CreateDelegate<Func<TObject, SealValue[], SealValue>>();
				return (self, args) => func(self.AsSealObject<TObject>(), args);
			}
			else
			{
				var func = methodInfo.CreateDelegate<Func<TObject, SealValue>>();
				return (self, _) => func(self.AsSealObject<TObject>());
			}
		}
	}
	
	private static PrototypeDataType ParseDataType(string type)
	{
		int scope = type.IndexOf("::", StringComparison.InvariantCulture);

		if (scope == -1)
		{
			return new PrototypeDataType(SourceLocation.Native, null, type);
		}

		string namespaceName = type[..scope];
		string className = type[(scope + 2)..];

		return new PrototypeDataType(SourceLocation.Native, namespaceName, className);
	}
	
	private static PrototypeArgumentList CreateArgumentList(
		FunctionExportAttribute exportAttribute,
		FunctionInfoAttribute infoAttribute)
	{
		string[] types = exportAttribute.ParameterTypes;

		int length = types.Length;
		
		var args = new PrototypeArgument[length];

		string[] names = infoAttribute?.ParameterNames ?? [];
		
		for (int i = 0; i < length; i++)
		{
			PrototypeDataType dataType = ParseDataType(types[i]);

			string name = i < names.Length ? names[i] : $"_p{i}";
			
			args[i] = new PrototypeArgument(name, dataType, true);
		}

		return new PrototypeArgumentList(args, exportAttribute.MinArgs, exportAttribute.MaxArgs);
	}
}