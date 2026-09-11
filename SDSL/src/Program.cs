using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL;

internal static class Program
{
    private static void Main(string[] args)
    {
#if DEBUG
        Run(args);
        return;
#endif
        
        try
        {
            Run(args);
        }
        catch (RuntimeException ex)
        {
            PrintError($"Runtime error {ex.Message}");
        }
        catch (ParserException ex)
        {
            PrintError($"Parsing error {ex.Message}");
        }
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        Console.ResetColor();
    }

    private static void Run(string[] args)
    {
        string directory = args.Length >= 1
            ? args[0]
            : Directory.GetCurrentDirectory();

        var pAssembly = new PrototypeAssembly("Assembly");

        pAssembly.GlobalUsings.Add(GlobalConfig.Global);

        // Create prototypes for Native and Standard Library classes e.g. Number, String, Math
        SealClassFactory.GenerateNativeClasses(pAssembly);
        
        // Create prototypes for User classes (all .sdsl files in the current directory)
        PrototypeParser.ParseProjectDirectory(pAssembly, directory);
        
        // Allocate -> Build Classes -> Generate Members
        SealAssembly assembly = new AssemblyGenerator(pAssembly).GenerateAssembly();
        
        assembly.InvokeMain(args);
    }
}