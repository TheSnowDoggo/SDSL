using System.Text;

namespace SDSL;

public abstract class Property
{
	public string Name { get; protected init; }
	
	public VariantClass LocalClass { get; protected init; }
	
	public string PrototypeValueClass { get; set; }
	
	// Setter is public to allow native classes to resolve the class later
	public VariantClass ValueClass { get; set; }
	
	public abstract bool IsStatic { get; }

	public string FullName => $"{LocalClass.Name}.{Name}";

	public Variant MemberGet(Variant self)
	{
		if (IsStatic)
		{
			throw new RuntimeException($"Cannot get static property {FullName} in a non-static context.");
		}
		
		if (!self.IsAssignableTo(LocalClass))
		{
			throw new RuntimeException($"Member property {FullName} expected self parameter to be assignable to {LocalClass}, got {self.Class}.");
		}

		return Get(self);
	}
	
	public Variant StaticGet()
	{
		if (!IsStatic)
		{
			throw new InvalidOperationException($"Cannot get member property {FullName} in a static context.");
		}
		
		return Get(Variant.Nil);
	}
	
	protected abstract Variant Get(Variant self);

	public void MemberSet(Variant self, Variant value)
	{
		if (IsStatic)
		{
			throw new RuntimeException($"Cannot get static property {FullName} in a non-static context.");
		}
		
		if (!self.IsAssignableTo(LocalClass))
		{
			throw new RuntimeException($"Member property {FullName} expected self parameter to be assignable to {LocalClass}, got {self.Class}.");
		}
		
		if (!value.IsAssignableTo(ValueClass))
		{
			throw new RuntimeException($"Member property {FullName} expected value to be assignable to {ValueClass}, got {value.Class}.");
		}

		Set(self, value);
	}

	public void StaticSet(Variant value)
	{
		if (!IsStatic)
		{
			throw new RuntimeException($"Cannot get member property {FullName} in a static context.");
		}
		
		if (!value.IsAssignableTo(ValueClass))
		{
			throw new RuntimeException($"Static property {FullName} expected value to be assignable to {ValueClass}, got {value.Class}.");
		}
		
		Set(Variant.Nil, value);
	}
	
	protected abstract void Set(Variant self, Variant value);

	public override string ToString()
	{
		var sb = new StringBuilder();

		if (IsStatic)
		{
			sb.Append("static ");
		}

		sb.Append("var ");

		sb.Append(LocalClass);
		sb.Append('.');
		sb.Append(Name);

		sb.Append(": ");

		sb.Append(ValueClass == null ? "Any" : ValueClass);
		
		return sb.ToString();
	}
}