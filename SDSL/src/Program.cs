using System.Reflection;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

internal static class Program
{
    private const string ProjectDirectory = "/home/luna-sparkle/RiderProjects/SDSL/SDSL/scripts";
    
    private static void Main(string[] args)
    {
        var pAssembly = new PrototypeAssembly("Assembly");
        
        // Generate Core classes under the global namespace e.g. Nil, Number
        PrototypeClassFactory.GenerateNativeClasses(pAssembly);

        // Generate Standard library classes e.g. Math, Console
        PrototypeClassFactory.GenerateExportedClasses(
            pAssembly,
            Assembly.GetCallingAssembly()
        );
        
        // Implicit using global;
        pAssembly.GlobalUsings.Add(LangConfig.Global);

        // Tokenize and Prototype Parse every .sdsl file in the project
        foreach (string file in Directory.EnumerateFiles(
            ProjectDirectory, "*.sdsl", SearchOption.AllDirectories))
        {
            Token[] tokens = Tokenizer.TokenizeFile(file);
            
            new PrototypeParser(
                new TokenStream(tokens),
                pAssembly
            ).Parse();
        }

        // Assembly generation has multiple stages:
        // 1 - Allocation: Map every Function and Field to an index
        // This must be done before any expression parsing so static references
        // can be resolved into indexes
        // 2 - Function Parsing: Parses every static/instance function
        // 3 - Instance field parsing: Parses the expressions for instance fields
        // 4 - Static field parsing: Parses and Evaluates static fields
        SealAssembly assembly = pAssembly.GenerateAssembly();
        
        assembly.EntryPoint?.Invoke();
    }
}