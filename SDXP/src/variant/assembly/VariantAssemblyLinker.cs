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
		
		var userClassStack = new Stack<UserVariantClass>();
		
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
				bool success = propertyMap.TryAdd(property.Name, property);
				
				if (property.IsStatic)
				{
					continue;
				}

				if (!success)
				{
					throw new NativeFactoryException(
						$"Class {variantClass} : Found duplicate instance property {property.Name}, virtual instance properties are disallowed.");
				}
			}

			foreach (Constant constant in currentClass.DeclaredConstants)
			{
				constantMap.TryAdd(constant.Name, constant);
			}
			
			// Is the class a user class and has not been allocated yet
			if (currentClass is UserVariantClass { InstanceFields: null } userVariantClass)
			{
				// Resolve prototype base class type
				LinkBaseClass(userVariantClass);
				
				// Add to the allocation stack
				userClassStack.Push(userVariantClass);
			}
			
			currentClass = currentClass.BaseClass;
		}

		variantClass.BaseClassSet = baseClassSet.ToFrozenSet();
		
		variantClass.FunctionMap = functionMap.ToFrozenDictionary();
		variantClass.PropertyMap = propertyMap.ToFrozenDictionary();
		variantClass.ConstantMap = constantMap.ToFrozenDictionary();
		
		AllocateInstanceFields(userClassStack);
	}

	private static void AllocateInstanceFields(Stack<UserVariantClass> userClassStack)
	{
		if (userClassStack.Count == 0)
		{
			return;
		}

		UserVariantClass firstClass = userClassStack.Peek();

		var instanceFields = new List<UserInstanceProperty>();

		// If the first class in the stack is a user class, it must have already been allocated
		// We need to offset the field location from the base clase
		if (firstClass.BaseClass is UserVariantClass baseClass)
		{
			instanceFields.AddRange(baseClass.InstanceFields);
		}

		while (userClassStack.TryPop(out UserVariantClass userClass))
		{
			foreach (Property property in userClass.DeclaredProperties)
			{
				if (property.IsStatic)
				{
					continue;
				}

				var userProperty = (UserInstanceProperty)property;

				userProperty.FieldLocation = instanceFields.Count;
				
				instanceFields.Add(userProperty);
			}
			
			// Each class gets its own array with the instance fields
			userClass.InstanceFields = instanceFields.ToArray();
		}
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