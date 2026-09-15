using SDSL.Expressions;

namespace SDSL;

public class UserConstructor : Function
{
	private readonly UserVariantClass _userVariantClass;
	
	public UserConstructor(
		UserVariantClass variantClass,
		UserFunction userFunction)
	{
		Name = "new";
		
		_userVariantClass = variantClass;
		
		IsStatic = true;
		Signature = userFunction?.Signature ?? FunctionSignature.Empty;

		UserFunction = userFunction;
	}

	public override VariantClass DeclaredClass => _userVariantClass;
	
	public UserFunction UserFunction { get; }
	
	public ArraySegment<Token> BaseCallTokens { get; set; }
	public Expression[] BaseCallArgumentList { get; set; } = [];
	
	public NativeVariantClass CompositeClass { get; set; }
	
	protected override Variant Invoke(Variant self, Variant[] args)
	{
		var instance = new UserVariantObject(_userVariantClass);
		
		CreateFields(instance);

		Construct(instance, args);
		
		return instance;
	}

	private VariantObject CreateCompositeBase(Variant[] args)
	{
		if (CompositeClass == null)
		{
			return null;
		}

		Variant value;
		
		try
		{
			value = CompositeClass.Constructor.StaticInvoke(args);
		}
		catch (Exception ex)
		{
			throw new RuntimeException(
				$"Failed to instantiate composite class {CompositeClass}.\n  --> {ex.Message}", ex);
		}

		if (!value.IsAssignableTo(CompositeClass))
		{
			throw new RuntimeException(UserFunction,
				$"Expected composite class object to be of type {CompositeClass}, got {value.Class}.");
		}

		return value.AsVariantObject();
	}

	private void CreateFields(UserVariantObject userVariantObject)
	{
		UserInstanceProperty[] instanceProperties = _userVariantClass.InstanceFields;
		
		int length = instanceProperties.Length;

		if (length == 0)
		{
			userVariantObject.Fields = [];
			return;
		}
		
		Variant[] fields = new Variant[length];

		for (int i = 0; i < length; i++)
		{
			UserInstanceProperty property = instanceProperties[i];

			try
			{
				fields[i] = property.Expression?.Evaluate(null)
				    ?? VariantClass.GetDefaultValue(property.ValueClass);
			}
			catch (Exception ex)
			{
				throw new RuntimeException(UserFunction, 
					$"Failed to initialize field {property.FullName}.\n  --> {ex.Message}", ex);
			}
		}

		userVariantObject.Fields = fields;
	}

	private void Construct(Variant self, Variant[] args)
	{
		if (UserFunction == null)
		{
			VariantClass baseClass = _userVariantClass.BaseClass;
			
			if (baseClass == null)
			{
				return;
			}

			if (baseClass.Constructor != null)
			{
				throw new RuntimeException(
					"Cannot invoke implicit constructor as base class has defined a constructor.");
			}
			
			if (baseClass is UserVariantClass userBaseClass)
			{
				userBaseClass.UserConstructor.Construct(self, args);
			}
			
			return;
		}

		UserFunction.ValidateArguments(args);
		
		Variable[] variables = UserFunction.InitializeVariables(self, args);

		Variant[] nextArgs = EvaluateArgs(variables);

		if (CompositeClass != null)
		{
			self.AsVariantObject<UserVariantObject>().CompositeBase = CreateCompositeBase(nextArgs);
		}
		else if (_userVariantClass.BaseClass is UserVariantClass nextBaseClass)
		{
			nextBaseClass.UserConstructor.Construct(self, nextArgs);
		}

		UserFunction.UnsafeInvoke(variables);
	}

	private Variant[] EvaluateArgs(Variable[] variables)
	{
		int length = BaseCallArgumentList.Length;
		
		if (length == 0)
		{
			return [];
		}

		var args = new Variant[length];

		for (int i = 0; i < length; i++)
		{
			args[i] = BaseCallArgumentList[i].Evaluate(variables);
		}

		return args;
	}
}