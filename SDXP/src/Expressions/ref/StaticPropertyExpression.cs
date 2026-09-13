namespace SDSL.Expressions;

public class StaticPropertyExpression : AssignableExpression
{
	private readonly Property _property;

	public StaticPropertyExpression(
		SourceLocation location,
		Property property)
	{
		Location = location;
		_property = property;
	}

	public override Variant Evaluate(Variable[] variables)
	{
		return _property.Get(Variant.Nil);
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		_property.Set(Variant.Nil, value);
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