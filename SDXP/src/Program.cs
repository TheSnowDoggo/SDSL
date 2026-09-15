namespace SDSL;

internal static class Program
{
	private const string FilePath = @"/home/luna-sparkle/RiderProjects/SDXP/SDXP/scripts/program.sdxp";
	
	private static void Main(string[] args)
	{
		var assembly = new VariantAssembly();
		
		VariantClassFactory.GenenerateNativeAssembly(assembly);

		var linker = new VariantAssemblyLinker(assembly);

		linker.LinkNativeClasses();

		Token[] tokens;

		using (Tokenizer tokenizer = new Tokenizer(File.OpenText(FilePath)))
		{
			tokens = tokenizer.Tokenize();
		}

		TokenStream stream = new TokenStream(tokens);
		
		new ClassParser(assembly, stream).Parse();
		
		linker.LinkUserClasses();
		
		new VariantAssemblyGenerator(assembly).GenerateMembers();

		assembly.Classes["Program"].FunctionMap["main"].StaticInvoke();
	}
}