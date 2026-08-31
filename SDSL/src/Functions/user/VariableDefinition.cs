namespace SDSL.Functions;

public readonly struct VariableDefinition
{
	public VariableDefinition(string name, bool isConst)
	{
		Name = name;
		IsConst = isConst;
	}
	
	public string Name { get; }
	public bool IsConst { get; }
}