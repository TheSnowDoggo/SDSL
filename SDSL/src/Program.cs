using SDSL.Prototypes;

namespace SDSL;

internal static class Program
{
    private static void Main(string[] args)
    {
        const string FilePath = "/home/luna-sparkle/RiderProjects/SDSL/SDSL/scripts/main.sdsl";

        Token[] tokens;
        using (var tokenizer = new Tokenizer(File.OpenRead(FilePath)))
        {
            tokens = tokenizer.Tokenize();
        }

        var stream = new TokenStream(tokens);

        var prototypeAssembly = new PrototypeAssembly("Assembly");

        var prototypeParser = new PrototypeParser(stream, prototypeAssembly);
        
        prototypeParser.Parse();
        
        prototypeAssembly.GenerateAssembly();

        var curNamespace = prototypeAssembly.Namespaces["Project"];
        var curClass = curNamespace.Classes["Program"];
        
        Console.WriteLine(curClass.Constructor);
        Console.WriteLine(string.Join('\n', curClass.Fields.Values));
        Console.WriteLine(string.Join('\n', curClass.Functions.Values));
    }
}