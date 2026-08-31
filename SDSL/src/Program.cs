using System.Diagnostics;
using System.Reflection;
using SDSL.Prototypes;

namespace SDSL;

internal static class Program
{
    private const string ProjectDirectory = @"C:\Users\redst\RiderProjects\SDSL\SDSL\scripts";
    
    private static void Main(string[] args)
    {
        DebugRun();
        return;
        
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
        string directory;

        if (args.Length >= 1)
        {
            directory = args[0];
        }
        else
        {
            directory = Directory.GetCurrentDirectory();
        }
            
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
            string name = Path.GetRelativePath(directory, file);

            Token[] tokens;
            using (var tokenizer = new Tokenizer(File.OpenText(file), name))
            {
                tokens = tokenizer.Tokenize();
            }
            
            new PrototypeParser(
                new TokenStream(tokens),
                pAssembly
            ).Parse();
        }
        
        pAssembly.GenerateAssembly();
        
        SealAssembly.Current.RunMain(args);
    }

    private static void DebugRun()
    {
        Run([ProjectDirectory]);
    }
}