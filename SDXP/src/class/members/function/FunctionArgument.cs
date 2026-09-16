using System.Text;

namespace SDSL;

public class FunctionArgument
{
	public FunctionArgument(
		string name,
		string prototypeClass)
	{
		Name = name;
		PrototypeClass = prototypeClass;
	}
	
	public string Name { get; }
	
	public string PrototypeClass { get; set; }
	public VariantClass VariantClass { get; set; }

	public Variant DefaultValue { get; set; }
	
	public override string ToString()
	{
		var sb = new StringBuilder();

		sb.Append(Name);
		sb.Append(": ");

		sb.Append(VariantClass == null ? "Any" : VariantClass);

		if (DefaultValue.VariantType != VariantType.Nil)
		{
			sb.Append(" = ");
			sb.Append(DefaultValue);
		}
		
		return sb.ToString();
	}
}