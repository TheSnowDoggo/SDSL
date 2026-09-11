namespace SDSL.Expressions;

public class LocalRefExpression : AssignableExpression
{
	private readonly int _index;

	public LocalRefExpression(SourceLocation location, int index)
	{
		Location = location;
		_index = index;
	}

	public int Index => _index;

	public override SealValue Evaluate(Variable[] variables)
	{
		return variables[_index].Value;
	}

	public override void SetValue(Variable[] variables, SealValue value)
	{
		ref Variable variable = ref variables[_index];
		
		if (!value.Class.IsAssignableTo(variable.Class))
		{
			throw new RuntimeException(Location,
				$"Value {value.Class} is not assignable to variable {ToString()} of class {variable.Class}.");
		}

		variable.Value = value;
	}

	public override bool IsConstantEval()
	{
		return false;
	}

	public override string ToString()
	{
		return $"local_{_index}";
	}
}