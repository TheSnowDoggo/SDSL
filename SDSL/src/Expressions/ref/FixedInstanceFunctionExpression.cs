namespace SDSL.Expressions;

public class FixedInstanceFunctionExpression : Expression,
	IMemberFunctionExpression
{
	private readonly Expression _selfExpression;
	private readonly Function _function;
	
	public FixedInstanceFunctionExpression(
		Expression selfExpression,
		Function function)
	{
		_selfExpression = selfExpression;
		_function = function;
	}
	
	public override Variant Evaluate(Variable[] variables)
	{
		return _function;
	}

	public FunctionInfo GetFunctionInfo(Variable[] variables)
	{
		Variant self = _selfExpression.Evaluate(variables);
		
		return new FunctionInfo(self, _function);
	}

	public override string ToString()
	{
		return $"{_selfExpression}.{_function.Name}";
	}
}