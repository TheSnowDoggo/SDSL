using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SDSL;

public static class NativeClassFactory
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
	
	public delegate NativeFunctionInvoke MethodBinder(MethodInfo methodInfo, FunctionFlags flags);

	public abstract class PropertyBinder
	{
		public abstract NativePropertyGetter BindGetter(MethodInfo getMethodInfo);
		public abstract NativePropertySetter BindSetter(MethodInfo setMethodInfo);
	}

	private sealed class ObjectPropertyBinder<TObject> : PropertyBinder
		where TObject : VariantObject
	{
		private ObjectPropertyBinder() { }
		
		public static ObjectPropertyBinder<TObject> Instance { get; } = new ObjectPropertyBinder<TObject>();
		
		public override NativePropertyGetter BindGetter(MethodInfo getMethodInfo)
		{
			var func = getMethodInfo.CreateDelegate<Func<TObject, Variant>>();
			return self => func(self.NativeCast<TObject>());
		}

		public override NativePropertySetter BindSetter(MethodInfo setMethodInfo)
		{
			var action = setMethodInfo.CreateDelegate<Action<TObject, Variant>>();
			return (self, value) => action(self.NativeCast<TObject>(), value);
		}
	}

	public static void GenerateAssembly(
		VariantAssembly variantAssembly,
		Assembly assembly)
	{
		Type[] exportedTypes = assembly.GetExportedTypes();

		object[] args = [variantAssembly];
		
		for (int i = 0; i < exportedTypes.Length; i++)
		{
			Type type = exportedTypes[i];

			ClassExportAttribute attribute = type.GetCustomAttribute<ClassExportAttribute>();

			if (attribute == null)
			{
				continue;
			}

			string generatorName = attribute.GenerateMethodName ?? DefaultGenerateMethodName;

			MethodInfo methodInfo = type.GetMethod(generatorName);

			if (methodInfo == null)
			{
				throw new NativeFactoryException(
					$"Native Class {type} : Generate method with name '{generatorName}' not found.");
			}
			
			ValidateGenerateMethod(methodInfo);			

			try
			{
				methodInfo.Invoke(null, args);
			}
			catch (Exception ex)
			{
				throw new NativeFactoryException(
					$"Native Generate {methodInfo.Name} : Failed to generate, {ex.Message}", ex);
			}
		}
	}

	public static void GenenerateNativeAssembly(VariantAssembly variantAssembly)
	{
		GenerateAssembly(variantAssembly, Assembly.GetAssembly(typeof(NativeClassFactory)));
	}

	public static void GenerateClass(
		VariantAssembly variantAssembly,
		Type type,
		NativeClass nativeClass,
		[AllowNull] MethodBinder instanceMethodBinder = null,
		[AllowNull] PropertyBinder instancePropertyBinder = null)
	{
		AddClass(variantAssembly, type, nativeClass);
		
		BindMethods(type, nativeClass, instanceMethodBinder);
		
		BindProperties(type, nativeClass, instancePropertyBinder);
		
		BindConstants(type, nativeClass);
	}

	public static void GenerateClass<TObject>(
		VariantAssembly variantAssembly,
		NativeClass nativeClass)
		where TObject : VariantObject
	{
		GenerateClass(variantAssembly, typeof(TObject), nativeClass,
			BindInstanceMethod<TObject>, ObjectPropertyBinder<TObject>.Instance);
	}

	public static void GenerateEnum(
		VariantAssembly variantAssembly,
		Type enumType,
		NativeClass nativeClass)
	{
		AddClass(variantAssembly, enumType, nativeClass);
		
		if (!enumType.IsEnum)
		{
			throw new NativeFactoryException($"Native Enum {enumType} : Type must be an enum.");
		}

		string[] names = enumType.GetEnumNames();
		Array values = enumType.GetEnumValues();

		int length = names.Length;

		for (int i = 0; i < length; i++)
		{
			string name = names[i];
			
			object obj = values.GetValue(i);
			double value = Convert.ToDouble(obj);
			
			nativeClass.CreateConstant(name, value);
		}
	}

	private static bool TryGetCustomAttribute<TAttribute>(this MethodInfo methodInfo, out TAttribute attribute)
		where TAttribute : Attribute
	{
		attribute = methodInfo.GetCustomAttribute<TAttribute>();
		return attribute != null;
	}
	
	private static void ValidateGenerateMethod(MethodInfo methodInfo)
	{
		ParameterInfo[] parameters = methodInfo.GetParameters();
		
		if (parameters.Length != 1)
		{
			throw new NativeFactoryException(
				$"Native Generate {methodInfo.Name} : Method must take {typeof(VariantAssembly)} as the only parameter, got no parameters.");
		}

		if (parameters[0].ParameterType != typeof(VariantAssembly))
		{
			throw new NativeFactoryException(
				$"Native Generate {methodInfo.Name} : Method must take {typeof(VariantAssembly)} as the only parameter, got {parameters[0].ParameterType}.");
		}

		if (methodInfo.ReturnType != typeof(void))
		{
			throw new NativeFactoryException(
				$"Native Generate {methodInfo.Name} : Method must return {typeof(void)}.");
		}
	}

	private static void AddClass(
		VariantAssembly variantAssembly,
		Type type,
		NativeClass nativeClass)
	{
		if (!variantAssembly.Classes.TryAdd(nativeClass.Name, nativeClass))
		{
			throw new NativeFactoryException(
				$"Native Class {type} : Class with name {nativeClass.Name} has already been defined.");
		}

		variantAssembly.NativeClasses.Add(nativeClass);
	}
	
	public static bool HasFunctionFlags(this FunctionFlags self, FunctionFlags flags)
	{
		return (self & flags) == flags;
	}

	private static void BindMethods(
		Type type,
		NativeClass nativeClass,
		[AllowNull] MethodBinder instanceMethodBinder)
	{
		MethodInfo[] methods = type.GetMethods(
			BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

		for (int i = 0; i < methods.Length; i++)
		{
			MethodInfo methodInfo = methods[i];

			if (methodInfo.TryGetCustomAttribute(out FunctionExportAttribute fAttribute))
			{
				BindFunction(nativeClass, methodInfo, instanceMethodBinder, fAttribute);
			}
			
			if (methodInfo.TryGetCustomAttribute(out GetterFunctionExportAttribute pfAttribute))
			{
				BindPropertyFunction(nativeClass, methodInfo, pfAttribute);
			}
		}
	}

	private static FunctionFlags GetFunctionFlags(MethodInfo methodInfo)
	{
		return GetParameterFlags(methodInfo) | GetReturnFlags(methodInfo);
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

			if (firstType == typeof(Variant))
			{
				if (!methodInfo.IsStatic)
				{
					throw new NativeFactoryException($"Native Function {methodInfo.Name} : Function with self parameter must be static.");
				}
				
				return FunctionFlags.Self;
			}
			
			if (firstType == typeof(Variant[]))
			{
				return FunctionFlags.Args;
			}
			
			throw new NativeFactoryException(
				$"Native Function {methodInfo.Name} : First parameter must be of type {typeof(Variant[])} or {typeof(Variant)}, got {firstType}.");
		}
		case 2:
			if (!methodInfo.IsStatic)
			{
				throw new NativeFactoryException($"Native Function {methodInfo.Name} : Function with self parameter must be static.");
			}
			
			if (parameterInfos[0].ParameterType != typeof(Variant))
			{
				throw new NativeFactoryException($"Native Function {methodInfo.Name} : Self parameter must be of type {typeof(Variant)}, got {parameterInfos[0].ParameterType}.");
			}
			
			if (parameterInfos[1].ParameterType != typeof(Variant[]))
			{
				throw new NativeFactoryException($"Native Function {methodInfo.Name} : Args parameter must be of type {typeof(Variant[])}, got {parameterInfos[1].ParameterType}.");
			}

			return FunctionFlags.Args | FunctionFlags.Self;
		default:
			throw new NativeFactoryException($"Native Function {methodInfo.Name} : Function must take 0 or 1 parameter(s), got {parameterInfos.Length}.");
		}
	}
	
	private static FunctionFlags GetReturnFlags(MethodInfo methodInfo)
	{
		Type returnType = methodInfo.ReturnType;
		
		if (returnType == typeof(void))
		{
			return FunctionFlags.Void;
		}
		
		if (returnType == typeof(Variant))
		{
			return FunctionFlags.None;
		}

		throw new NativeFactoryException($"Native Function {methodInfo.Name} : Must return {typeof(Variant)} or {typeof(void)}, got {returnType}.");
	}

	private static NativeFunctionInvoke BindMethodInvoke(
		MethodInfo methodInfo,
		[AllowNull] MethodBinder instanceMethodBinder,
		out bool isStatic)
	{
		isStatic = methodInfo.IsStatic;

		FunctionFlags flags = GetFunctionFlags(methodInfo);

		if (isStatic)
		{
			if (!flags.HasFlag(FunctionFlags.Self))
			{
				return BindStaticMethod(methodInfo, flags);
			}
		
			isStatic = false;

			return BindStaticSelfMethod(methodInfo, flags);
		}
		else
		{
			if (instanceMethodBinder == null)
			{
				throw new NativeFactoryException($"Native Function {methodInfo.Name} : Instance method was exported but no instance method binder was set.");
			}

			return instanceMethodBinder(methodInfo, flags);
		}
	}

	private static void BindFunction(
		NativeClass nativeClass,
		MethodInfo methodInfo,
		[AllowNull] MethodBinder instanceMethodBinder,
		FunctionExportAttribute exportAttribute)
	{
		ValidateFunctionExportAttribute(methodInfo, exportAttribute);

		string name = exportAttribute.Name ?? methodInfo.Name;

		if (!nativeClass.MemberNames.Add(name))
		{
			throw new NativeFactoryException(
				$"Native Function {methodInfo.Name} : Member with name {name} has already been defined.");
		}

		NativeFunctionInvoke invoke = BindMethodInvoke(methodInfo, instanceMethodBinder, out bool isStatic);

		FunctionInfoAttribute infoAttribute = methodInfo.GetCustomAttribute<FunctionInfoAttribute>();
			
		FunctionSignature signature = CreateFunctionSignature(exportAttribute, infoAttribute);

		NativeFunction function = new NativeFunction(
			name,
			nativeClass,
			isStatic,
			signature,
			invoke
		);

		if (exportAttribute is not ConstructorExportAttribute)
		{
			nativeClass.LocalNativeFunctions.Add(function);
			return;
		}

		if (!isStatic)
		{
			throw new NativeFactoryException(
				$"Native Function {methodInfo.Name} : Constructor must be static.");
		}

		if (nativeClass.Constructor != null)
		{
			throw new NativeFactoryException(
				$"Native Function {methodInfo.Name} : Constructor {nativeClass.Constructor.FullName} has already been defined.");
		}

		nativeClass.NativeConstructor = function;
	}
	
	private static void ValidateFunctionExportAttribute(MethodInfo methodInfo, FunctionExportAttribute attribute)
	{
		if (attribute.MinArgs < 0)
		{
			throw new NativeFactoryException(
				$"Native Function {methodInfo.Name} : Attribute minimum args {attribute.MinArgs} was negative.");
		}
		
		if (attribute.MaxArgs >= 0 && attribute.MinArgs > attribute.MaxArgs)
		{
			throw new NativeFactoryException(
				$"Native Function {methodInfo.Name} : Attribute minimum args {attribute.MinArgs} was greater than maximum args {attribute.MaxArgs}.");
		}
	}
	
	private static NativeFunctionInvoke BindStaticMethod(MethodInfo methodInfo, FunctionFlags flags)
	{
		if (flags.HasFunctionFlags(FunctionFlags.Void))
		{
			if (flags.HasFunctionFlags(FunctionFlags.Args))
			{
				var action = methodInfo.CreateDelegate<Action<Variant[]>>();
				return (_, args) => { action(args); return Variant.Nil; };
			}
			else
			{
				var action = methodInfo.CreateDelegate<Action>();
				return (_, _) => { action(); return Variant.Nil; };
			}
		}
		else
		{
			if (flags.HasFunctionFlags(FunctionFlags.Args))
			{
				var func = methodInfo.CreateDelegate<Func<Variant[], Variant>>();
				return (_, args) => func(args);
			}
			else
			{
				var func = methodInfo.CreateDelegate<Func<Variant>>();
				return (_, _) => func();
			}
		}
	}

	private static NativeFunctionInvoke BindStaticSelfMethod(MethodInfo methodInfo, FunctionFlags flags)
	{
		if (flags.HasFunctionFlags(FunctionFlags.Void))
		{
			if (flags.HasFunctionFlags(FunctionFlags.Args))
			{
				var action = methodInfo.CreateDelegate<Action<Variant, Variant[]>>();
				return (self, args) => { action(self, args); return Variant.Nil; };
			}
			else
			{
				var action = methodInfo.CreateDelegate<Action<Variant>>();
				return (self, _) => { action(self); return Variant.Nil; };
			}
		}
		else
		{
			if (flags.HasFunctionFlags(FunctionFlags.Args))
			{
				var func = methodInfo.CreateDelegate<Func<Variant, Variant[], Variant>>();
				return (self, args) => func(self, args);
			}
			else
			{
				var func = methodInfo.CreateDelegate<Func<Variant, Variant>>();
				return (self, _) => func(self);
			}
		}
	}

	private static NativeFunctionInvoke BindInstanceMethod<TObject>(MethodInfo methodInfo, FunctionFlags flags)
		where TObject : VariantObject
	{
		if (flags.HasFunctionFlags(FunctionFlags.Void))
		{
			if (flags.HasFunctionFlags(FunctionFlags.Args))
			{
				var action = methodInfo.CreateDelegate<Action<TObject, Variant[]>>();
				return (self, args) => { action(self.NativeCast<TObject>(), args); return Variant.Nil; };
			}
			else
			{
				var action = methodInfo.CreateDelegate<Action<TObject>>();
				return (self, _) => { action(self.NativeCast<TObject>()); return Variant.Nil; };
			}
		}
		else
		{
			if (flags.HasFunctionFlags(FunctionFlags.Args))
			{
				var func = methodInfo.CreateDelegate<Func<TObject, Variant[], Variant>>();
				return (self, args) => func(self.NativeCast<TObject>(), args);
			}
			else
			{
				var func = methodInfo.CreateDelegate<Func<TObject, Variant>>();
				return (self, _) => func(self.NativeCast<TObject>());
			}
		}
	}
	
	private static FunctionSignature CreateFunctionSignature(
		FunctionExportAttribute exportAttribute,
		[AllowNull] FunctionInfoAttribute infoAttribute)
	{
		string[] argumentTypes = exportAttribute.ArgumentTypes;

		int length = argumentTypes.Length;
		
		FunctionArgument[] arguments = new FunctionArgument[length];

		string[] names = infoAttribute?.ArgumentNames ?? [];

		for (int i = 0; i < length; i++)
		{
			string name = i < names.Length ? names[i] : $"arg_{i + 1}";
			
			arguments[i] = new FunctionArgument(name, argumentTypes[i]);
		}

		return new FunctionSignature(
			arguments,
			exportAttribute.MinArgs,
			exportAttribute.MaxArgs,
			exportAttribute.ReturnType
		);
	}

	private static void BindPropertyFunction(
		NativeClass nativeClass,
		MethodInfo methodInfo,
		GetterFunctionExportAttribute attribute)
	{
		string name = attribute.Name ?? methodInfo.Name;

		if (!nativeClass.MemberNames.Add(name))
		{
			throw new NativeFactoryException(
				$"Native Function Property {methodInfo.Name} : Member with name {name} has already been defined.");
		}

		if (!methodInfo.IsStatic)
		{
			throw new NativeFactoryException(
				$"Native Function Property {methodInfo.Name} : Method must be static.");
		}

		if (methodInfo.ReturnType != typeof(Variant))
		{
			throw new NativeFactoryException(
				$"Native Function Property {methodInfo.Name} : Getter method must return {typeof(Variant)}, got {methodInfo.ReturnType}.");
		}
		
		ParameterInfo[] parameters = methodInfo.GetParameters();

		if (parameters.Length != 1)
		{
			throw new NativeFactoryException(
				$"Native Function Property {methodInfo.Name} : Method take 1 or 2 parameters, got {parameters.Length}.");
		}
		
		if (parameters[0].ParameterType != typeof(Variant))
		{
			throw new NativeFactoryException(
				$"Native Function Property {methodInfo.Name} : First parameter 'self' must be of type {typeof(Variant)}, got {parameters[0].ParameterType}.");
		}

		var getter = methodInfo.CreateDelegate<NativePropertyGetter>();

		var property = new NativeProperty(
			name,
			nativeClass,
			attribute.ValueType,
			false,
			getter,
			null
		);
		
		nativeClass.LocalNativeProperties.Add(property);
	}

	private static void BindProperties(Type type,
		NativeClass nativeClass,
		[AllowNull] PropertyBinder instancePropertyBinder)
	{
		PropertyInfo[] properties = type.GetProperties(
			BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

		for (int i = 0; i < properties.Length; i++)
		{
			PropertyInfo propertyInfo = properties[i];

			PropertyExportAttribute attribute = propertyInfo.GetCustomAttribute<PropertyExportAttribute>();

			if (attribute == null)
			{
				continue;
			}
			
			string name = attribute.Name ?? propertyInfo.Name;

			if (!nativeClass.MemberNames.Add(name))
			{
				throw new NativeFactoryException(
					$"Native Property {propertyInfo.Name} : Member with name {name} has already been defined.");
			}
			
			if (propertyInfo.PropertyType != typeof(Variant))
			{
				throw new NativeFactoryException(
					$"Native Property {propertyInfo.Name} : Exported property must be of type {typeof(Variant)}, got {propertyInfo.PropertyType}.");
			}

			NativePropertyGetter getter = BindGetter(propertyInfo, instancePropertyBinder, out bool isStatic);
			
			NativePropertySetter setter = BindSetter(propertyInfo, instancePropertyBinder);

			NativeProperty property = new NativeProperty(
				name,
				nativeClass,
				attribute.ValueType,
				isStatic,
				getter,
				setter
			);
			
			nativeClass.LocalNativeProperties.Add(property);
		}
	}

	private static NativePropertyGetter BindGetter(
		PropertyInfo propertyInfo,
		[AllowNull] PropertyBinder instancePropertyBinder,
		out bool isStatic)
	{
		MethodInfo getMethodInfo = propertyInfo.GetGetMethod();

		if (getMethodInfo == null)
		{
			throw new NativeFactoryException(
				$"Native Property {propertyInfo.Name} : Property must have a getter.");
		}

		isStatic = getMethodInfo.IsStatic;

		if (isStatic)
		{
			var func = getMethodInfo.CreateDelegate<Func<Variant>>();
			return _ => func();
		}

		if (instancePropertyBinder == null)
		{
			throw new NativeFactoryException(
				$"Native Property {propertyInfo.Name} : Instance property was exported but no property binder was set.");
		}

		return instancePropertyBinder.BindGetter(getMethodInfo);
	}

	private static NativePropertySetter BindSetter(
		PropertyInfo propertyInfo,
		[AllowNull] PropertyBinder instancePropertyBinder)
	{
		MethodInfo setMethodInfo = propertyInfo.GetSetMethod();

		if (setMethodInfo == null)
		{
			return null;
		}

		if (setMethodInfo.IsStatic)
		{
			var action = setMethodInfo.CreateDelegate<Action<Variant>>();
			return (_, value) => action(value);
		}
		
		// Instance property binder will have already been validated by the getter binder.
		
		return instancePropertyBinder.BindSetter(setMethodInfo);
	}

	private static void BindConstants(
		Type type,
		NativeClass nativeClass)
	{
		FieldInfo[] fields = type.GetFields(
			BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);

		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];

			ConstantExportAttribute attribute = fieldInfo.GetCustomAttribute<ConstantExportAttribute>();

			if (attribute == null)
			{
				continue;
			}
			
			string name = attribute.Name ?? fieldInfo.Name;

			if (!nativeClass.MemberNames.Add(name))
			{
				throw new NativeFactoryException(
					$"Native Field {fieldInfo.Name} : Member with name {name} has already been defined.");
			}

			object obj = fieldInfo.GetValue(null);

			Variant value = Variant.FromObject(obj);

			nativeClass.CreateConstant(name, value);
		}
	}
}