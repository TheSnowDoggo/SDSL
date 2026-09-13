namespace SDSL;

internal static class Program
{
	private static void Main(string[] args)
	{
		var variantAssembly = new VariantAssembly();
		
		VariantClassFactory.GenenerateNativeAssembly(variantAssembly);

		var linker = new VariantAssemblyLinker(variantAssembly);

		linker.LinkNativeClasses();
	}
}