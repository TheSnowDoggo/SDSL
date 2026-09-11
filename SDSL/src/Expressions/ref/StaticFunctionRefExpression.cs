namespace SDSL.Expressions;

public class StaticFunctionRefExpression : Expression
{
	private readonly SealAssembly _assembly;
	private readonly int _index;

	public StaticFunctionRefExpression(
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
		return _assembly.StaticFunctions[_index];
	}

	public override bool IsConstantEval()
	{
		return false;
	}

	public override string ToString()
	{
		return $"static_function_{_index}";
	}
}