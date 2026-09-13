namespace SDSL.Expressions;

public class InstancePropertyExpression : AssignableExpression
{
	private readonly Expression _instanceExpression;
	private readonly Property _property;
	
	public InstancePropertyExpression(
		SourceLocation location,
		Expression instanceExpression,
		Property property)
	{
		Location = location;
		_instanceExpression = instanceExpression;
		_property = property;
	}

	public override Variant Evaluate(Variable[] variables)
	{
		Variant self = _instanceExpression.Evaluate(variables);

		return _property.Get(self);
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		Variant self = _instanceExpression.Evaluate(variables);
		
		_property.Set(self, value);
	}

	public override bool IsConstantEval()
	{
		return false;
	}
	
	public override string ToString()
	{
		return $"{_instanceExpression}.{_property.Name}";
	}
}