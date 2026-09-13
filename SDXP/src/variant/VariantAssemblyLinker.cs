using System.Collections.Frozen;

namespace SDSL;

public class VariantAssemblyLinker
{
	private readonly VariantAssembly _assembly;

	public VariantAssemblyLinker(VariantAssembly assembly)
	{
		_assembly = assembly;
	}
	
	public void LinkNativeClasses()
	{
		foreach (VariantClass variantClass in _assembly.NativeClasses)
		{
			// Linking every function first is not necessary
			BuildInheritanceTree(variantClass);
			
			LinkNativeFunctions(variantClass);

			LinkNativeProperties(variantClass);
		}
	}

	private static void BuildInheritanceTree(VariantClass variantClass)
	{
		var baseClassSet = new HashSet<VariantClass>();
		
		var functionMap = new Dictionary<string, Function>();
		var propertyMap = new Dictionary<string, Property>();
		var constantMap = new Dictionary<string, Constant>();
		
		VariantClass currentClass = variantClass;

		while (currentClass != null)
		{
			if (!baseClassSet.Add(currentClass))
			{
				throw new NativeFactoryException(
					$"Native Class {variantClass} : Found recursive base class {currentClass}.");
			}
			
			// Both Static and Instance functions declared in a higher class will shadow
			// any function with the same name in a base class allowing for virtual and shadowed functions
			foreach (Function function in currentClass.DeclaredFunctions)
			{
				functionMap.TryAdd(function.Name, function);
			}

			foreach (Property property in currentClass.DeclaredProperties)
			{
				if (propertyMap.TryAdd(property.Name, property))
				{
					continue;
				}

				if (!property.IsStatic)
				{
					throw new NativeFactoryException(
						$"Native Class {variantClass} : Found duplicate instance property {property.Name}, virtual instance properties are disallowed.");
				}
			}

			foreach (Constant constant in currentClass.DeclaredConstants)
			{
				constantMap.TryAdd(constant.Name, constant);
			}
			
			currentClass = currentClass.BaseClass;
		}

		variantClass.BaseClassSet = baseClassSet.ToFrozenSet();
		
		variantClass.FunctionMap = functionMap.ToFrozenDictionary();
		variantClass.PropertyMap = propertyMap.ToFrozenDictionary();
		variantClass.ConstantMap = constantMap.ToFrozenDictionary();
	}

	private void LinkNativeFunctions(VariantClass variantClass)
	{
		foreach (Function function in variantClass.DeclaredFunctions)
		{
			LinkNativeArguments(function);
			LinkReturnType(function);
		}
	}

	private void LinkNativeArguments(Function function)
	{
		FunctionArgument[] arguments = function.Signature.Arguments;
			
		for (int i = 0; i < arguments.Length; i++)
		{
			FunctionArgument argument = arguments[i];

			string className = argument.PrototypeClass;

			VariantClass argumentClass;

			if (className is null or "Any")
			{
				argumentClass = null;
			}
			else if (!_assembly.Classes.TryGetValue(className, out argumentClass))
			{
				throw new NativeFactoryException(
					$"Native Function {function.FullName} : Argument {i + 1} had unknown native type '{className}'.");
			}

			argument.VariantClass = argumentClass;
			argument.PrototypeClass = null;
		}
	}

	private void LinkReturnType(Function function)
	{
		FunctionSignature signature = function.Signature;

		string pReturnType = signature.PrototypeReturnType;
			
		VariantClass returnType;

		if (pReturnType is null or "Any")
		{
			returnType = null;
		}
		else if (!_assembly.Classes.TryGetValue(pReturnType, out returnType))
		{
			throw new NativeFactoryException(
				$"Function {function.FullName} : Return type '{pReturnType}' not found.");
		}

		signature.ReturnType = returnType;
		signature.PrototypeReturnType = null;
	}

	private void LinkNativeProperties(VariantClass variantClass)
	{
		foreach (Property property in variantClass.DeclaredProperties)
		{
			var nativeProperty = (NativeProperty)property;

			string pValueClass = nativeProperty.PrototypeValueClass;
			
			VariantClass valueClass;

			if (pValueClass is null or "Any")
			{
				valueClass = null;
			}
			else if (!_assembly.Classes.TryGetValue(pValueClass, out valueClass))
			{
				throw new NativeFactoryException(
					$"Native Property {property.FullName} : Had unknown native value type '{pValueClass}'.");
			}

			nativeProperty.ValueClass = valueClass;

			// No longer needed
			nativeProperty.PrototypeValueClass = null;
		}
	}
}