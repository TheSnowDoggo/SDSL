using System.Diagnostics.CodeAnalysis;

namespace SDSL;

public delegate Variant NativePropertyGetter(Variant self);

public delegate void NativePropertySetter(Variant self, Variant value);

public class NativeProperty : Property
{
	public NativeProperty(
		string name,
		VariantClass declaredClass,
		string prototypeValueClass,
		bool isStatic)
	{
		Name = name;
		DeclaredClass = declaredClass;
		PrototypeValueClass = prototypeValueClass;
		IsStatic = isStatic;
	}
	
	public override bool IsStatic { get; }
	
	public NativePropertyGetter Getter { get; set; }
	public NativePropertySetter Setter { get; set; }
	
	protected override Variant Get(Variant self)
	{
		return Getter(self);
	}

	protected override void Set(Variant self, Variant value)
	{
		if (Setter == null)
		{
			throw new InvalidOperationException($"Native Property {Name} is readonly.");
		}

		Setter(self, value);
	}
}