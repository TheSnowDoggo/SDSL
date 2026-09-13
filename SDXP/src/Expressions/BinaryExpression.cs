namespace SDSL.Expressions;

public abstract class BinaryExpression : Expression
{
	protected Expression Left { get; init; }
	protected Expression Right { get; init; }

	public override bool IsConstantEval()
	{
		return Left.IsConstantEval() && Right.IsConstantEval();
	}
}