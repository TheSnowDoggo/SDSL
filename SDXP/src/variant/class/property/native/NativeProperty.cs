using System.Diagnostics.CodeAnalysis;

namespace SDSL;

public delegate Variant NativePropertyGetter(Variant self);

public delegate void NativePropertySetter(Variant self, Variant value);

public class NativeProperty : Property
{
	private readonly NativePropertyGetter _getter;
	private readonly NativePropertySetter _setter;

	public NativeProperty(
		string name,
		VariantClass variantClass,
		string prototypeValueClass,
		bool isStatic,
		NativePropertyGetter getter,
		[AllowNull] NativePropertySetter setter)
	{
		Name = name;
		VariantClass = variantClass;
		PrototypeValueClass = prototypeValueClass;
		IsStatic = isStatic;
		_getter = getter;
		_setter = setter;
	}
	
	public string PrototypeValueClass { get; set; }
	
	public override Variant Get(Variant self)
	{
		return _getter(self);
	}

	public override void Set(Variant self, Variant value)
	{
		if (_setter == null)
		{
			throw new InvalidOperationException($"Native Property {Name} is readonly.");
		}

		_setter(self, value);
	}
}