namespace SDSL;

public readonly struct FunctionInfo
{
	public FunctionInfo(Variant self, Function function)
	{
		Self = self;
		Function = function;
	}
	
	public Variant Self { get; }
	public Function Function { get; }

	public void Deconstruct(out Variant self, out Function function)
	{
		self = Self;
		function = Function;
	}
}