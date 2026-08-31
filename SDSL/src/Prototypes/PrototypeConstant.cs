using System.Text;

namespace SDSL.Prototypes;

public class PrototypeConstant
{
	public PrototypeConstant(
		PrototypeClass pClass,
		string name,
		ArraySegment<Token> tokens)
	{
		Class = pClass;
		Name = name;
		Tokens = tokens;
	}
	
	public PrototypeClass Class { get; }
	public string Name { get; }
	public ArraySegment<Token> Tokens { get; }

	public int AssemblyLocation { get; set; } = -1;

	public override string ToString()
	{
		var sb = new StringBuilder();

		sb.Append("constexpr ");
		sb.Append(Name);

		if (Tokens.Count != 0)
		{
			sb.Append(" = ");
			sb.Append($"Expression[{Tokens.Count}]");
		}
        
		return sb.ToString();
	}
}