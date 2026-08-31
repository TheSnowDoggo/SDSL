namespace SDSL.Expressions;

public abstract class BinaryExpression : Expression
{
	public Expression Left { get; protected init; }
	public Expression Right { get; protected init; }

	public override bool IsConstantEval()
	{
		return Left.IsConstantEval() && Right.IsConstantEval();
	}
}