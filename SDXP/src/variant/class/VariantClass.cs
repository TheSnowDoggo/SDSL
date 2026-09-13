using System.Collections.Frozen;

namespace SDSL;

[ClassExport]
public class VariantClass : VariantObject
{
	public VariantClass(string name, VariantType variantType)
	{
		Name = name;
		VariantType = variantType;
	}

	public static VariantClass TypeClass { get; } = new VariantClass("Type", VariantType.Object);

	public override VariantClass Class => TypeClass;

	public string Name { get; }
	
	public VariantType VariantType { get; }

	// Default to inheriting from Object
	public VariantClass BaseClass { get; set; } = ObjectClass.Class;

	public FrozenSet<VariantClass> BaseClassSet { get; set; } = FrozenSet<VariantClass>.Empty;
	
	public Function[] DeclaredFunctions { get; set; } = [];
	public FrozenDictionary<string, Function> FunctionMap { get; set; } = FrozenDictionary<string, Function>.Empty;

	public Property[] DeclaredProperties { get; set; } = [];
	public FrozenDictionary<string, Property> PropertyMap { get; set; } = FrozenDictionary<string, Property>.Empty;

	public Constant[] DeclaredConstants { get; set; } = [];
	public FrozenDictionary<string, Constant> ConstantMap { get; set; } = FrozenDictionary<string, Constant>.Empty;

	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass<VariantClass>(variantAssembly, TypeClass);
	}

	[PropertyExport("String")]
	public Variant name => Name;
	
	public override string ToString()
	{
		return Name;
	}
}