using System.Collections.Frozen;
using SDSL.Expressions;

namespace SDSL.Statements;

public class SwitchStatement : Statement
{
	private readonly Expression _expression;
	private readonly FrozenDictionary<SealValue, BlockStatement> _blocks;
	private readonly BlockStatement _defaultBlock;
	
	public SwitchStatement(
		SourceLocation location,
		Expression expression,
		FrozenDictionary<SealValue, BlockStatement> blocks,
		BlockStatement defaultBlock)
	{
		Location = location;
		_expression = expression;
		_blocks = blocks;
		_defaultBlock = defaultBlock;
	}
	
	public override ReturnValue Invoke(Variable[] variables)
	{
		SealValue value = _expression.Evaluate(variables);

		if (_blocks.TryGetValue(value, out BlockStatement blockStatement))
		{
			return blockStatement.Invoke(variables);
		}

		if (_defaultBlock != null)
		{
			return _defaultBlock.Invoke(variables);
		}

		return ReturnValue.None;
	}
}