using System.Diagnostics;
using System.Reflection;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

internal static class Program
{
    private const string ProjectDirectory = "/home/luna-sparkle/RiderProjects/SDSL/SDSL/scripts";
    
    private static void Main(string[] args)
    {
        try
        {
            Run(args);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(ex.Message);
            Console.ResetColor();
            
            Console.Read();
        }
    }

    private static void Run(string[] args)
    {
        if (args.Length < 1)
            args = [Directory.GetCurrentDirectory()];
        
        string directory = args[0];
            
        var pAssembly = new PrototypeAssembly("Assembly");

        // Generate Native and Standard Library classes e.g. Number, String, Math
        PrototypeClassFactory.GenerateExportedClasses(
            pAssembly,
            Assembly.GetCallingAssembly()
        );

        // Implicit using global;
        pAssembly.GlobalUsings.Add(LangConfig.GlobalNamespace);

        // Tokenize and Prototype Parse every .sdsl file in the project
        foreach (string file in Directory.EnumerateFiles(
            directory, "*.sdsl", SearchOption.AllDirectories))
        {
            string name = Path.GetRelativePath(ProjectDirectory, file);
            
            Token[] tokens = new Tokenizer(File.OpenText(file), name).Tokenize();
            
            new PrototypeParser(
                new TokenStream(tokens),
                pAssembly
            ).Parse();
        }
        
        pAssembly.GenerateAssembly();
        
        SealAssembly.Current.Run(args);
    }

    private static void DebugRun()
    {
        Run([ProjectDirectory]);
    }
}