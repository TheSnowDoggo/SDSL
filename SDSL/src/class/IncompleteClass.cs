namespace SDSL;

public sealed class IncompleteClass : VariantClass
{
	public const string ImplicitName = "<implicit>";
	
	private IncompleteClass()
	{
		Name = ImplicitName;
	}

	public static IncompleteClass Implicit { get; } = new IncompleteClass();

	public override Function Constructor => null;

	public override IReadOnlyList<Function> LocalFunctions => null;

	public override IReadOnlyList<Property> LocalProperties => null;

	public override IReadOnlyList<Constant> LocalConstants => null;
}