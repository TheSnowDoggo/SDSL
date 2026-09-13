using System.Text;

namespace SDSL;

public abstract class Property
{
	public string Name { get; protected init; }
	
	public VariantClass VariantClass { get; protected init; }
	
	// Setter is public to allow native classes to resolve the class later
	public VariantClass ValueClass { get; set; }
	
	public bool IsStatic { get; protected init; }

	public string FullName => $"{VariantClass.Name}.{Name}";
	
	public abstract Variant Get(Variant self);

	public abstract void Set(Variant self, Variant value);

	public override string ToString()
	{
		var sb = new StringBuilder();

		if (IsStatic)
		{
			sb.Append("static ");
		}

		sb.Append("var ");

		sb.Append(VariantClass);
		sb.Append('.');
		sb.Append(Name);

		sb.Append(": ");

		sb.Append(ValueClass == null ? "Any" : ValueClass);
		
		return sb.ToString();
	}
}