namespace SDSL;

public sealed class IncompleteVariantClass : VariantClass
{
	public const string ImplicitName = "<implicit>";
	
	private IncompleteVariantClass()
	{
		Name = ImplicitName;
	}

	public static IncompleteVariantClass Implicit { get; } = new IncompleteVariantClass();

	public override Function Constructor => null;
}