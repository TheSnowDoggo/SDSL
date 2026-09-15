using SDSL.Expressions;

namespace SDSL;

public class VariantAssemblyGenerator
{
	private readonly VariantAssembly _assembly;
	private readonly Queue<(UserStaticProperty, Expression)> _staticInitializeQueue = [];

	public VariantAssemblyGenerator(VariantAssembly assembly)
	{
		_assembly = assembly;
	}

	public void GenerateMembers()
	{
		foreach (UserVariantClass variantClass in _assembly.UserClasses)
		{
			GenerateConstructor(variantClass);
			
			GenerateFunctions(variantClass);
			
			GenerateProperties(variantClass);
		}

		InitializeStaticProperties();
	}

	private void GenerateFunctions(UserVariantClass variantClass)
	{
		foreach (Function function in variantClass.DeclaredFunctions)
		{
			new FunctionParser(_assembly, (UserFunction)function).Parse();
		}
	}

	private void GenerateConstructor(UserVariantClass userClass)
	{
		UserConstructor constructor = userClass.UserConstructor;
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
	
	private void GenerateProperties(UserVariantClass variantClass)
	{
		foreach (Property property in variantClass.DeclaredProperties)
		{
			var userProperty = (UserProperty)property;

			Expression expression = null;

			if (userProperty.Tokens.Count > 0)
			{
				expression = new ExpressionParser(_assembly, variantClass, null,
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

			if (property.ValueClass == ImplicitVariantClass.Instance)
			{
				property.ValueClass = defaultValue.Class;
			}

			property.Value = defaultValue;
		}
	}
}