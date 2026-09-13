namespace SDSL;

public class VariantAssembly
{
	public List<NativeVariantClass> NativeClasses { get; } = [];
	public List<UserVariantClass> UserClasses { get; } = [];

	public Dictionary<string, VariantClass> Classes { get; } = [];
}