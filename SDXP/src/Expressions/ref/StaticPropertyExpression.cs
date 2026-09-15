namespace SDSL.Expressions;

public class StaticPropertyExpression : AssignableExpression
{
	private readonly Property _property;

	public StaticPropertyExpression(Property property)
	{
		_property = property;
	}

	public override Variant Evaluate(Variable[] variables)
	{
		return _property.StaticGet();
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		_property.StaticSet(value);
	}

	public override bool IsConstantEval()
	{
		return false;
	}
	
	public override string ToString()
	{
		return _property.FullName;
	}
}