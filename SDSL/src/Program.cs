using System.Reflection;
using SDSL.Prototypes;

namespace SDSL;

internal static class Program
{
    private const string ProjectDirectory = @"C:\Users\redst\RiderProjects\SDSL\SDSL\scripts";
    
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
        }
    }

    private static void Run(string[] args)
    {
        string directory = ProjectDirectory;
            
        var pAssembly = new PrototypeAssembly("Assembly");

        // Generate Native and Standard Library classes e.g. Number, String, Math
        PrototypeClassFactory.GenerateExportedClasses(
            pAssembly,
            Assembly.GetCallingAssembly()
        );

        // Implicit using global;
        pAssembly.GlobalUsings.Add(GlobalConfig.GlobalNamespace);

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