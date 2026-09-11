namespace SDSL.Expressions;

public class StaticFieldRefExpression : AssignableExpression
{
	private readonly SealAssembly _assembly;
	private readonly int _index;

	public StaticFieldRefExpression(
		SourceLocation location,
		SealAssembly assembly,
		int index)
	{
		Location = location;
		_assembly = assembly;
		_index = index;
	}
	
	public override SealValue Evaluate(Variable[] variables)
	{
		return _assembly.StaticFields[_index].Value;
	}

	public override void SetValue(Variable[] variables, SealValue value)
	{
		ref Field field = ref _assembly.StaticFields[_index];
		
		if (field.IsConst)
		{
			throw new RuntimeException(Location,
				$"Field {ToString()} cannot be assigned to as it is const.");
		}

		if (!value.Class.IsAssignableTo(field.Class))
		{
			throw new RuntimeException(Location,
				$"Value {value.Class} is not assignable to field {ToString()} of class {field.Class}.");
		}
            
		field.Value = value;
	}

	public override bool IsConstantEval()
	{
		return false;
	}
	
	public override string ToString()
	{
		return $"static_field_{_index}";
	}
}