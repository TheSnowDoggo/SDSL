using System.Collections.Frozen;

namespace SDSL;

[ClassExport]
public abstract class VariantClass : VariantObject
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("Type");

	public override VariantClass ParentClass => Class;

	public string Name { get; protected init;  }
	
	public VariantType VariantType { get; protected init; }

	public VariantClass BaseClass { get; set; }

	public abstract Function Constructor { get; }

	public FrozenSet<VariantClass> BaseClassSet { get; set; } = FrozenSet<VariantClass>.Empty;
	
	public List<Function> DeclaredFunctions { get; set; } = [];
	public List<Property> DeclaredProperties { get; set; } = [];
	public List<Constant> DeclaredConstants { get; set; } = [];
	
	public FrozenDictionary<string, Function> FunctionMap { get; set; } = FrozenDictionary<string, Function>.Empty;
	public FrozenDictionary<string, Property> PropertyMap { get; set; } = FrozenDictionary<string, Property>.Empty;
	public FrozenDictionary<string, Constant> ConstantMap { get; set; } = FrozenDictionary<string, Constant>.Empty;
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass<VariantClass>(variantAssembly, Class);
	}

	public static Variant GetDefaultValue(VariantClass variantClass)
	{
		if (variantClass == null)
		{
			return Variant.Nil;
		}
		
		return variantClass.VariantType switch
		{
			VariantType.Nil      => Variant.Nil,
			VariantType.Bool     => false,
			VariantType.Number   => 0,
			VariantType.DateTime => default(DateTime),
			VariantType.TimeSpan => TimeSpan.Zero,
			VariantType.String   => string.Empty,
			VariantType.Object   => Variant.Nil,
			_ => throw new InvalidOperationException($"Class had invalid Variant type {variantClass.VariantType}."),
		};
	}

	[PropertyExport("String", Name = "name")]
	public Variant _name => Name;
	
	public override string ToString()
	{
		return Name;
	}
}