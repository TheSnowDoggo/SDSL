namespace SDSL;

public sealed class ImplicitVariantClass : VariantClass
{
	public const string ImplicitName = "<implicit>";
	
	private ImplicitVariantClass()
	{
		Name = ImplicitName;
	}

	public static ImplicitVariantClass Instance { get; } = new ImplicitVariantClass();
}