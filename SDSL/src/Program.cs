using System.Reflection;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL;

internal static class Program
{
    private static void Main(string[] args)
    {
        try
        {
            Run(args);
        }
        catch (NativeFactoryException ex)
        {
            PrintError($"Native library exception: {ex.Message}");
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

        var pAssembly = new PrototypeAssembly("Assembly")
        {
            GlobalUsings = [GlobalConfig.GlobalNamespace],
        };

        // Generate Native and Standard Library classes e.g. Number, String, Math
        SealClassFactory.GenerateNativeClasses(pAssembly);
        
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

            TokenStream stream = new TokenStream(tokens);
            
            new PrototypeParser(stream, pAssembly).Parse();
        }
        
        pAssembly.GenerateAssembly();
        
        SealAssembly.Current.InvokeMain(args);
    }
}