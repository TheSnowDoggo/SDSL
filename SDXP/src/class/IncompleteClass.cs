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
}