using System.Globalization;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[CustomClassGenerator]
public static class SealNumber
{
    public const string Name = "global::Number";
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Number", ValueType.Number);

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealNumber), pAssembly, Class);
    }
    
    [SealConstructor]
    [SealFunctionExport("Any")]
    public static SealValue _new(SealValue[] args)
    {
        SealValue value = args[0];

        return value.ValueType switch
        {
            ValueType.Nil    => 0,
            ValueType.Bool   => value.AsBool() ? 1 : 0,
            ValueType.Number => value,
            ValueType.String => 
                double.TryParse(value.AsString(), out double parsedValue)
                    ? parsedValue
                    : SealValue.Nil,
            _ => 0,
        };
    }

    [SealFunctionExport("String", MinArgs = 0)]
    public static SealValue to_string(SealValue self, SealValue[] args) => args.Length switch
    {
        0 => self.AsNumber().ToString(CultureInfo.InvariantCulture),
        1 => self.AsNumber().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };
}