using SDSL.Expressions;

namespace SDSL;

public class AssemblyGenerator
{
	private readonly VariantAssembly _assembly;
	private readonly Queue<(UserStaticProperty, Expression)> _staticInitializeQueue = [];

	public AssemblyGenerator(VariantAssembly assembly)
	{
		_assembly = assembly;
	}

	public void GenerateMembers()
	{
		foreach (UserClass variantClass in _assembly.UserClasses)
		{
			GenerateConstructor(variantClass);
			
			GenerateFunctions(variantClass);
			
			GenerateProperties(variantClass);
		}

		InitializeStaticProperties();
	}

	private void GenerateFunctions(UserClass userClass)
	{
		foreach (Function function in userClass.DeclaredFunctions)
		{
			new FunctionParser(_assembly, (UserFunction)function).Parse();
		}
	}

	private void GenerateConstructor(UserClass userClass)
	{
		UserConstructor constructor = userClass.UserConstructor;

		if (constructor == null)
		{
			return;
		}
		
		UserFunction function = constructor.UserFunction;
		
		if (function == null)
		{
			return;
		}

		var parser = new FunctionParser(_assembly, function);
		
		parser.DefineArguments();

		ArraySegment<Token> baseCallTokens = constructor.BaseCallTokens;

		if (baseCallTokens.Count != 0)
		{
			constructor.BaseCallArgumentList = new ExpressionParser(_assembly, userClass, parser,
				ExpressionParsingMode.Statement, new TokenStream(baseCallTokens)).ParseFullArgumentList();

			constructor.BaseCallTokens = ArraySegment<Token>.Empty;
		}
		
		parser.ParseStatements();
	}
	
	private void GenerateProperties(UserClass userClass)
	{
		foreach (Property property in userClass.DeclaredProperties)
		{
			var userProperty = (UserProperty)property;

			Expression expression = null;

			if (userProperty.Tokens.Count > 0)
			{
				expression = new ExpressionParser(_assembly, userClass, null,
					ExpressionParsingMode.Statement, new TokenStream(userProperty.Tokens)).Parse();
			}
			
			if (property.IsStatic)
			{
				_staticInitializeQueue.Enqueue(((UserStaticProperty)property, expression));
			}
			else
			{
				var instanceProperty = (UserInstanceProperty)property;

				instanceProperty.Expression = expression;
			}
		}
	}

	private void InitializeStaticProperties()
	{
		while (_staticInitializeQueue.TryDequeue(out (UserStaticProperty, Expression) result))
		{
			(UserStaticProperty property, Expression expression) = result;

			Variant defaultValue;
			
			try
			{
				defaultValue = expression != null
					? expression.Evaluate(null)
					: VariantClass.GetDefaultValue(property.ValueClass);
			}
			catch (Exception ex)
			{
				throw new RuntimeException(property.Location, 
					$"Failed to initialize static field {property.Name}.\n  --> {ex.Message}", ex);
			}

			if (property.ValueClass == IncompleteClass.Implicit)
			{
				property.ValueClass = defaultValue.Class;
			}

			property.Value = defaultValue;
		}
	}
}