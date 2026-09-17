using SDSL.Expressions;

namespace SDSL;

public class UserConstructor : Function
{
	private readonly UserClass _userClass;
	
	public UserConstructor(
		UserClass userClass,
		UserFunction userFunction)
	{
		Name = "new";
		IsStatic = true;
		Signature = userFunction?.Signature ?? FunctionSignature.Empty;
		
		_userClass = userClass;

		UserFunction = userFunction;
	}

	public override VariantClass DeclaredClass => _userClass;
	
	public UserFunction UserFunction { get; }
	
	public ArraySegment<Token> BaseCallTokens { get; set; }
	public Expression[] BaseCallArgumentList { get; set; } = [];
	
	public NativeClass CompositeClass { get; set; }
	
	protected override Variant Invoke(Variant self, Variant[] args)
	{
		var instance = new UserObject(_userClass);
		
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

	private void CreateFields(UserObject userObject)
	{
		UserInstanceProperty[] instanceProperties = _userClass.InstanceFields;
		
		int length = instanceProperties.Length;

		if (length == 0)
		{
			userObject.Fields = [];
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

		userObject.Fields = fields;
	}

	private void Construct(Variant self, Variant[] args)
	{
		if (UserFunction == null)
		{
			VariantClass baseClass = _userClass.BaseClass;
			
			if (baseClass == null)
			{
				return;
			}

			if (baseClass.Constructor != null)
			{
				throw new RuntimeException(
					"Cannot invoke implicit constructor as base class has defined a constructor.");
			}
			
			if (baseClass is UserClass userBaseClass)
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
			self.AsVariantObject<UserObject>().CompositeBase = CreateCompositeBase(nextArgs);
		}
		else if (_userClass.BaseClass is UserClass nextBaseClass)
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