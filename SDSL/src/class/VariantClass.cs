using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using SDSL.Native;

namespace SDSL;

public abstract class VariantClass : VariantObject
{
	public static NativeClass Class { get; } = NativeClass.InheritObject("Type");

	public override VariantClass ObjectClass => Class;

	public string Name { get; protected init;  }
	
	public VariantType VariantType { get; protected init; }

	public VariantClass BaseClass { get; set; }

	public abstract Function Constructor { get; }
	
	public abstract IReadOnlyList<Function> LocalFunctions { get; }
	
	public abstract IReadOnlyList<Property> LocalProperties { get; }
	
	public abstract IReadOnlyList<Constant> LocalConstants { get; }

	public FrozenSet<VariantClass> InheritanceTree { get; set; } = FrozenSet<VariantClass>.Empty;

	public HashSet<string> MemberNames { get; set; } = [];
	
	public FrozenDictionary<string, Function> FunctionMap { get; set; } = FrozenDictionary<string, Function>.Empty;
	
	public FrozenDictionary<string, Property> PropertyMap { get; set; } = FrozenDictionary<string, Property>.Empty;
	
	public FrozenDictionary<string, Constant> ConstantMap { get; set; } = FrozenDictionary<string, Constant>.Empty;
	
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

	public bool IsAssignableTo([AllowNull] VariantClass variantClass)
	{
		// Null represents untyped/Any
		// The base class set contains the class itself
		return variantClass == null || InheritanceTree.Contains(variantClass) || 
		       VariantType == VariantType.Nil && variantClass.VariantType == VariantType.Object;
	}
	
	[PropertyExport("String", Name = "name")]
	public Variant _name => Name;

	[FunctionExport]
	public static Variant get_members()
	{
		return new PackedStringArray(Class.MemberNames.ToArray());
	}
	
	[FunctionExport("String")]
	public static Variant get_function(Variant[] args)
	{
		return Class.FunctionMap.TryGetValue(args[0].AsString(),
			out Function function) ? function : Variant.Nil;
	}
	
	public override string ToString()
	{
		return Name;
	}
}