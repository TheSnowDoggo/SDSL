namespace SDSL.Expressions;

public class MemberExpression : AssignableExpression, IMemberFunctionExpression
{
	private readonly Expression _selfExpression;
	private readonly string _identifier;
	
	public MemberExpression(
		Expression selfExpression,
		string identifier)
	{
		_selfExpression = selfExpression;
		_identifier = identifier;
	}
	
	public override Variant Evaluate(Variable[] variables)
	{
		Variant self = _selfExpression.Evaluate(variables);

		VariantClass variantClass = self.Class;

		if (variantClass.PropertyMap.TryGetValue(_identifier, out Property property))
		{
			return property.MemberGet(self);
		}
		
		if (variantClass.FunctionMap.TryGetValue(_identifier, out Function function))
		{
			return function;
		}

		throw new RuntimeException($"No member '{_identifier}' found in class {variantClass}.");
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		Variant self = _selfExpression.Evaluate(variables);
		
		VariantClass variantClass = self.Class;
		
		if (!variantClass.PropertyMap.TryGetValue(_identifier, out Property property))
		{
			throw new RuntimeException($"No property '{_identifier}' found in class {variantClass}.");
		}
		
		property.MemberSet(self, value);
	}

	public FunctionInfo GetFunctionInfo(Variable[] variables)
	{
		Variant self = _selfExpression.Evaluate(variables);

		VariantClass variantClass = self.Class;

		if (variantClass.FunctionMap.TryGetValue(_identifier, out Function function))
		{
			return new FunctionInfo(self, function);
		}

		if (!variantClass.PropertyMap.TryGetValue(_identifier, out Property property))
		{
			throw new RuntimeException($"No member '{_identifier}' found in class {variantClass}.");
		}

		Variant value = property.MemberGet(self);

		if (!value.TryAsVariantObject(out function))
		{
			throw new RuntimeException($"Cannot invoke non-invokable type {value.Class}.");
		}

		return new FunctionInfo(self, function);
	}

	public override bool IsConstantEval()
	{
		return false;
	}

	public override string ToString()
	{
		return $"{_selfExpression}.{_identifier}";
	}
}