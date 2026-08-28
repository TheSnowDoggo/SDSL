using System.Reflection;
using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

internal static class Program
{
    private static void Main(string[] args)
    {
        var pAssembly = new PrototypeAssembly("Assembly");
        
        PrototypeClassFactory.GenerateNativeClasses(pAssembly);

        PrototypeClassFactory.GenerateExportedClasses(
            pAssembly,
            Assembly.GetCallingAssembly()
        );
        
        pAssembly.GlobalUsings.Add(LangConfig.Global);
        
        Token[] tokens = Tokenizer.TokenizeFile("/home/luna-sparkle/RiderProjects/SDSL/SDSL/scripts/main.sdsl");

        new PrototypeParser(
            new TokenStream(tokens),
            pAssembly
        ).Parse();

        SealAssembly assembly = pAssembly.GenerateAssembly();
        
        assembly.EntryPoint?.Invoke();
    }
}