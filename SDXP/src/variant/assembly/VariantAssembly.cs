namespace SDSL;

public class VariantAssembly
{
	public List<NativeVariantClass> NativeClasses { get; } = [];
	public List<UserVariantClass> UserClasses { get; } = [];

	public Dictionary<string, VariantClass> Classes { get; } = [];
	
	public UserFunction EntryPoint { get; set; }

	public Variant InvokeMain(string[] args)
	{
		if (EntryPoint == null)
		{
			throw new RuntimeException("Cannot invoke entry point as none has been defined.");
		}
		
		if (EntryPoint.Signature.MaxArgs == 0)
		{
			return EntryPoint.StaticInvoke();
		}
		
		int length = args.Length;
		
		var argList = new List<Variant>(length);

		for (int i = 0; i < length; i++)
		{
			argList.Add(args[i]);
		}

		var array = new SealArray(argList);

		return EntryPoint.StaticInvoke(array);
	}
}