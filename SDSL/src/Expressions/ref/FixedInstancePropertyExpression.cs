namespace SDSL.Expressions;

public class FixedInstancePropertyExpression : AssignableExpression,
	IMemberFunctionExpression
{
	private readonly Expression _selfExpression;
	private readonly Property _property;
	
	public FixedInstancePropertyExpression(
		Expression selfExpression,
		Property property)
	{
		_selfExpression = selfExpression;
		_property = property;
	}

	public override Variant Evaluate(Variable[] variables)
	{
		Variant self = _selfExpression.Evaluate(variables);

		return _property.MemberGet(self);
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		Variant self = _selfExpression.Evaluate(variables);
		
		_property.MemberSet(self, value);
	}

	public FunctionInfo GetFunctionInfo(Variable[] variables)
	{
		Variant self = _selfExpression.Evaluate(variables);

		Variant value = _property.MemberGet(self);

		if (!value.TryAsVariantObject(out Function function))
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
		return $"{_selfExpression}.{_property.Name}";
	}
}