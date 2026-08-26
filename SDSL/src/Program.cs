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
        
        var @class = prototypeAssembly.Namespaces["Project"]
            .Classes["Program"];

        var prototypeFunction = @class.Functions["test"];
        
        var functionParser = new FunctionParser(prototypeFunction);
        
        Function function = functionParser.Parse();
    }
}