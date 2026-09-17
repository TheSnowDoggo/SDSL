namespace SDSL;

internal static class Program
{
	private static void Main(string[] args)
	{
		try
		{
			Run(args);
		}
		catch (ParserException ex)
		{
			PrintError($"[Parser] {ex.Message}");
		}
		catch (RuntimeException ex)
		{
			PrintError($"[Runtime] {ex.Message}");
		}
		catch (Exception ex)
		{
			PrintError($"[Unexpected error] {ex}");
		}
	}

	private static void PrintError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(message);
		Console.ResetColor();
	}

	private static void Run(string[] args)
	{
		var assembly = new VariantAssembly();

		NativeClassFactory.GenenerateNativeAssembly(assembly);
		
		var linker = new AssemblyLinker(assembly);

		linker.LinkNativeClasses();

		string directory = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

		ClassParser.ParseDirectory(assembly, directory);
		
		linker.LinkUserClasses();
		
		new AssemblyGenerator(assembly).GenerateMembers();

		assembly.InvokeMain(args);
	}
}