using System.Reflection;

namespace SDSL.Prototypes;

public static class SealClassFactory<TObject>
	where TObject : SealObject
{
	[Flags]
	private enum FunctionFlags
	{
		None   = 0,
		Args   = 1,
		Void   = 2,
	}
	
	public static void Generate(PrototypeAssembly pAssembly, SealClass sClass)
	{
		PrototypeNamespace pNamespace = pAssembly.GetOrCreateNamespace(sClass.Namespace);

		if (pNamespace.Classes.ContainsKey(sClass.Name))
		{
			throw new NativeFactoryException($"Namespace {pNamespace} already contains class with name '{sClass.Name}'.");
		}
		
		var pClass = new PrototypeClass(pNamespace, sClass);

		var memberNames = new HashSet<string>();
		
		BindMethods(pClass, memberNames);
		
		BindConstants(pClass, memberNames);
		
		pNamespace.AddClass(pClass);
	}
	
	private static void BindMethods(PrototypeClass pClass, HashSet<string> memberNames)
	{
		MethodInfo[] methodInfos = typeof(TObject).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

		for (int i = 0; i < methodInfos.Length; i++)
		{
			MethodInfo methodInfo = methodInfos[i];

			var attribute = methodInfo.GetCustomAttribute<SealFunctionExportAttribute>();

			if (attribute == null)
			{
				continue;
			}
			
			string name = attribute.Name ?? methodInfo.Name;

			if (!memberNames.Add(name))
			{
				throw new NativeFactoryException(
					$"Class {pClass} already contains a member with name '{name}'.");
			}
			
			FunctionFlags flags = GetFunctionFlags(methodInfo);

			bool isStatic = methodInfo.IsStatic;
			
			Func<SealValue, SealValue[], SealValue> func = isStatic
				? BindStaticMethod(methodInfo, flags)
				: BindInstanceMethod(methodInfo, flags);

			PrototypeArgumentList args = CreateArgumentList(attribute);
			
			var pFunction = new PrototypeFunction(
				SourceLocation.Invalid,
				pClass,
				name,
				args,
				PrototypeDataType.Any,
				isStatic,
				new NativeFunctionBody(func)
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
	
	private static void BindConstants(PrototypeClass pClass, HashSet<string> memberNames)
	{
		FieldInfo[] fieldInfos = typeof(TObject).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);

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

			var pConstant = new PrototypeConstant(
				SourceLocation.Native,
				name,
				value
			);
            
			pClass.NativeConstants.Add(pConstant);
		}
	}

	private static FunctionFlags GetParameterFlags(ParameterInfo[] parameterInfos)
	{
		switch (parameterInfos.Length)
		{
		case 0:
			return FunctionFlags.None;
		case 1:
			if (parameterInfos[0].ParameterType != typeof(SealValue[]))
			{
				throw new NativeFactoryException($"Native function paramater must be of type {typeof(SealValue[])}, got {parameterInfos[0].ParameterType}.");
			}

			return FunctionFlags.Args;
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

		flags |= GetParameterFlags(methodInfo.GetParameters());
		flags |= GetReturnFlags(methodInfo.ReturnType);

		return flags;
	}

	private static bool HasFlag(FunctionFlags source, FunctionFlags flag)
	{
		return (source & flag) == flag;
	}
	
	private static Func<SealValue, SealValue[], SealValue> BindStaticMethod(MethodInfo methodInfo, FunctionFlags functionFlags)
	{
		if (HasFlag(functionFlags, FunctionFlags.Void))
		{
			if (HasFlag(functionFlags, FunctionFlags.Args))
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
			if (HasFlag(functionFlags, FunctionFlags.Args))
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
	
	private static Func<SealValue, SealValue[], SealValue> BindInstanceMethod(MethodInfo methodInfo, FunctionFlags functionFlags)
	{
		if (HasFlag(functionFlags, FunctionFlags.Void))
		{
			if (HasFlag(functionFlags, FunctionFlags.Args))
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
			if (HasFlag(functionFlags, FunctionFlags.Args))
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
	
	private static PrototypeArgumentList CreateArgumentList(SealFunctionExportAttribute attribute)
	{
		string[] types = attribute.Types;

		int length = types.Length;
		
		var args = new PrototypeArgument[length];

		for (int i = 0; i < length; i++)
		{
			PrototypeDataType dataType = ParseDataType(types[i]);
			
			args[i] = new PrototypeArgument(string.Empty, dataType, true);
		}

		return new PrototypeArgumentList(args, attribute.MinArgs, attribute.MaxArgs);
	}
}