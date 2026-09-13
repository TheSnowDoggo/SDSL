namespace SDSL.Expressions;

public class LocalRefExpression : AssignableExpression
{
	private readonly int _index;

	public LocalRefExpression(int index)
	{
		_index = index;
	}

	public int Index => _index;

	public override Variant Evaluate(Variable[] variables)
	{
		return variables[_index].Value;
	}

	public override void SetValue(Variable[] variables, Variant value)
	{
		ref Variable variable = ref variables[_index];
		
		if (!value.IsAssignableTo(variable.VariableClass))
		{
			throw new RuntimeException($"Value {value.Class} is not assignable to variable {ToString()} of class {variable.VariableClass}.");
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