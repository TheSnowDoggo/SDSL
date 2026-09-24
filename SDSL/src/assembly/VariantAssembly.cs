using SDSL.Native;

namespace SDSL;

public class VariantAssembly
{
	public List<NativeClass> NativeClasses { get; } = [];
	public List<UserClass> UserClasses { get; } = [];

	public Dictionary<string, VariantClass> Classes { get; } = [];
	
	public UserFunction EntryPoint { get; set; }

	public Variant InvokeMain(params string[] args)
	{
		if (EntryPoint == null)
		{
			throw new RuntimeException("Cannot invoke entry point as none has been defined.");
		}
		
		if (EntryPoint.Signature.MaxArgs == 0)
		{
			return EntryPoint.StaticInvoke();
		}
		
		return EntryPoint.StaticInvoke(new PackedStringArray(args));
	}
}