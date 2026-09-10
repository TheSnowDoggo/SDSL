using System.Reflection;
using SDSL.Functions;

namespace SDSL.Prototypes;

public static class SealClassFactory
{
	[Flags]
	private enum FunctionFlags
	{
		None   = 0,
		Args   = 1,
		Void   = 2,
	}
	
	public static void Generate(
		Type type,
		PrototypeAssembly pAssembly,
		SealClass sClass,
		Func<MethodInfo, FunctionFlags, NativeDelegate> instanceBinder = null)
	{
		PrototypeClass pClass = pAssembly.CreateClass(sClass);
		
		var memberNames = new HashSet<string>();
		
		BindMethods(type, pClass, memberNames, instanceBinder);
		
		BindConstants(type, pClass, memberNames);
	}

	public static void Generate<TObject>(PrototypeAssembly pAssembly, SealClass sClass)
		where TObject : SealObject
	{
		Generate(typeof(TObject), pAssembly, sClass, BindInstanceMethod<TObject>);
	}
	
	private static void BindMethods(
		Type type,
		PrototypeClass pClass,
		HashSet<string> memberNames,
		Func<MethodInfo, FunctionFlags, NativeDelegate> instanceBinder = null)
	{
		MethodInfo[] methodInfos = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

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

			NativeDelegate func;

			if (isStatic)
			{
				func = BindStaticMethod(methodInfo, flags);
			}
			else
			{
				if (instanceBinder == null)
				{
					throw new NativeFactoryException(
						$"Got instance method {methodInfo} but no instance binder was assigned.");
				}

				func = instanceBinder(methodInfo, flags);
			}

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
	
	private static NativeDelegate BindStaticMethod(MethodInfo methodInfo, FunctionFlags functionFlags)
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
	
	private static NativeDelegate BindInstanceMethod<TObject>(MethodInfo methodInfo, FunctionFlags functionFlags)
		where TObject : SealObject
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