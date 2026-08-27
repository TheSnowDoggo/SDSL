using SDSL.Prototypes;
using SDSL.Statements;

namespace SDSL;

internal static class Program
{
    private static void Main(string[] args)
    {
        var prototypeAssembly = new PrototypeAssembly("Assembly");

        PrototypeNamespace global = prototypeAssembly.GetOrCreateNamespace("global");
        prototypeAssembly.GlobalUsings.Add("global");

        PrototypeClassFactory.Generate(typeof(SealNil), global, "Nil", SealClass.Nil);
        PrototypeClassFactory.Generate(typeof(SealBool), global, "Bool", SealClass.Bool);
        PrototypeClassFactory.Generate(typeof(SealNumber), global, "Number", SealClass.Number);
        PrototypeClassFactory.Generate(typeof(SealString), global, "String", SealClass.String);
        PrototypeClassFactory.Generate(typeof(SealFunction), global, "Functino", SealClass.Function);

        const string FilePath = "/home/luna-sparkle/RiderProjects/SDSL/SDSL/scripts/main.sdsl";

        Token[] tokens;
        using (var tokenizer = new Tokenizer(File.OpenRead(FilePath)))
        {
            tokens = tokenizer.Tokenize();
        }

        var stream = new TokenStream(tokens);

        var prototypeParser = new PrototypeParser(stream, prototypeAssembly);
        
        prototypeParser.Parse();
        
        prototypeAssembly.GenerateAssembly();

        int location = prototypeAssembly.Namespaces["Project"]
            .Classes["Program"]
            .Functions["test"]
            .AssemblyLocation;
            
        Function function = prototypeAssembly.Assembly.Functions[location];
        
        Console.Write(string.Join<Statement>('\n', ((UserFunction)function).Statements));

        return;
        
        SealValue result = function.Invoke(SealValue.Nil, 10.8);

        Console.WriteLine($"Result: {result}");
    }
}