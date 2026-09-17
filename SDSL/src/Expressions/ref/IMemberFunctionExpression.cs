namespace SDSL.Expressions;

public interface IMemberFunctionExpression
{
	FunctionInfo GetFunctionInfo(Variable[] variables);
}