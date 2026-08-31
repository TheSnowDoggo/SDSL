using System.Collections.Frozen;
using SDSL.Expressions;

namespace SDSL.Statements;

public class SwitchStatement : Statement
{
	public SwitchStatement(
		SourceLocation location,
		Expression expression,
		FrozenDictionary<SealValue, BlockStatement> blocks,
		BlockStatement defaultBlock)
	{
		Location = location;
		Expression = expression;
		Blocks = blocks;
		DefaultBlock = defaultBlock;
	}
	
	public Expression Expression { get; }
	public FrozenDictionary<SealValue, BlockStatement> Blocks { get; }
	public BlockStatement DefaultBlock { get; }
	
	public override ReturnValue Invoke(Variable[] variables)
	{
		SealValue value = Expression.Evaluate(variables);

		if (Blocks.TryGetValue(value, out BlockStatement blockStatement))
		{
			return blockStatement.Invoke(variables);
		}

		if (DefaultBlock != null)
		{
			return DefaultBlock.Invoke(variables);
		}

		return ReturnValue.None;
	}
}