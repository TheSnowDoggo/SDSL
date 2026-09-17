namespace SDSL.Expressions;

public class LocalRefExpression : AssignableExpression
{
	private readonly int _location;
	private readonly string _name;

	public LocalRefExpression(int location, string name)
	{
		_location = location;
		_name = name;
	}

	public static LocalRefExpression Self { get; } = new LocalRefExpression(0, "self");

	public override Variant Evaluate(Variable[] variables)
	{
		return variables[_location].Value;
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		ref Variable variable = ref variables[_location];
		
		if (!value.IsAssignableTo(variable.VariableClass))
		{
			throw new RuntimeException($"Value {value.Class} is not assignable to variable {_name} of class {variable.VariableClass}.");
		}

		variable.Value = value;
	}

	public override string ToString()
	{
		return _name;
	}
}