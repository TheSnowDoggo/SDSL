using System.Text;

namespace SDSL.Prototypes;

public class PrototypeConstant : ISourceLocated
{
	public PrototypeConstant(
		SourceLocation location,
		string name,
		SealValue value)
	{
		Location = location;
		Name = name;
		Value = value;
	}
	
	public SourceLocation Location { get; }
	
	public string Name { get; }
	
	public SealValue Value { get; }

	public override string ToString()
	{
		var sb = new StringBuilder();

		sb.Append("constexpr ");
		sb.Append(Name);
		sb.Append(" = ");
		sb.Append(Value);
		sb.Append(';');
        
		return sb.ToString();
	}
}