namespace SDSL.Expressions;

public class InstanceFunctionExpression : InvokableExpression
{
	private readonly Expression _instanceExpression;

	public InstanceFunctionExpression(
		SourceLocation location,
		Expression instanceExpression,
		Function function)
	{
		Location = location;
		_instanceExpression = instanceExpression;
		Function = function;
	}

	public Function Function { get; }

	public override Variant Evaluate(Variable[] variables)
	{
		return Function;
	}

	public override Variant Invoke(Variant[] args)
	{
		throw new NotImplementedException();
	}

	public Variant GetInstance(Variable[] variables)
	{
		return _instanceExpression.Evaluate(variables);
	}

	public override bool IsConstantEval()
	{
		return true;
	}

	public override string ToString()
	{
		return $"{_instanceExpression}.{Function.Name}";
	}
}