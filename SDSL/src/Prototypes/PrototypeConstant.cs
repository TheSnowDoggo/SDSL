using System.Text;

namespace SDSL.Prototypes;

public class PrototypeConstant
{
	public PrototypeConstant(
		PrototypeClass pClass,
		string name,
		SealValue value)
	{
		Class = pClass;
		Name = name;
		Value = value;
	}
	
	public PrototypeClass Class { get; }
	public string Name { get; }
	public SealValue Value { get; }

	public int AssemblyLocation { get; set; } = -1;

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