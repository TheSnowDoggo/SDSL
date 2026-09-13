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
		foreach (NativeVariantClass variantClass in _assembly.NativeClasses)
		{
			LinkFunctions(variantClass);

			LinkProperties(variantClass);
			
			// Linking every function first is not necessary
			BuildInheritanceTree(variantClass);
		}
	}

	public void LinkUserClasses()
	{
		foreach (UserVariantClass variantClass in _assembly.UserClasses)
		{
			LinkFunctions(variantClass);
			
			LinkProperties(variantClass);
			
			BuildInheritanceTree(variantClass);
		}
	}
	
	private void BuildInheritanceTree(VariantClass variantClass)
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
					$"Class {variantClass} : Found recursive base class {currentClass}.");
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
						$"Class {variantClass} : Found duplicate instance property {property.Name}, virtual instance properties are disallowed.");
				}
			}

			foreach (Constant constant in currentClass.DeclaredConstants)
			{
				constantMap.TryAdd(constant.Name, constant);
			}
			
			if (currentClass.BaseClass == null && currentClass is UserVariantClass userVariantClass)
			{
				LinkBaseClass(userVariantClass);
			}
			
			currentClass = currentClass.BaseClass;
		}

		variantClass.BaseClassSet = baseClassSet.ToFrozenSet();
		
		variantClass.FunctionMap = functionMap.ToFrozenDictionary();
		variantClass.PropertyMap = propertyMap.ToFrozenDictionary();
		variantClass.ConstantMap = constantMap.ToFrozenDictionary();
	}

	private void LinkFunctions(VariantClass variantClass)
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
					$"Function {function.FullName} : Argument {i + 1} had unknown native type '{className}'.");
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

	private void LinkProperties(VariantClass variantClass)
	{
		foreach (Property property in variantClass.DeclaredProperties)
		{
			string pValueClass = property.PrototypeValueClass;
			
			VariantClass valueClass;

			switch (pValueClass)
			{
			case "Any":
			case null:
				valueClass = null;
				break;
			case ImplicitVariantClass.ImplicitName:
				valueClass = ImplicitVariantClass.Instance;
				break;
			default:
				if (!_assembly.Classes.TryGetValue(pValueClass, out valueClass))
				{
					throw new NativeFactoryException(
						$"Property {property.FullName} : Had unknown native value type '{pValueClass}'.");
				}
				break;
			}

			property.ValueClass = valueClass;

			// No longer needed
			property.PrototypeValueClass = null;
		}
	}

	private void LinkBaseClass(UserVariantClass variantClass)
	{
		string pBaseClass = variantClass.PrototypeBaseClass;
		
		VariantClass baseClass;

		if (pBaseClass is null or "Any")
		{
			baseClass = ObjectClass.Class;
		}
		else if (!_assembly.Classes.TryGetValue(pBaseClass, out baseClass))
		{
			throw new NativeFactoryException(
				$"Class {variantClass.Name} : Base class '{pBaseClass}' not found.");
		}

		variantClass.BaseClass = baseClass;
		variantClass.PrototypeBaseClass = null;
	}
}