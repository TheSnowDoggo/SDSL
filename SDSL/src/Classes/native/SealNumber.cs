using System.Globalization;
using SDSL.Factory;
using SDSL.Prototypes;

namespace SDSL.Classes;

[ClassExport]
public static class SealNumber
{
    public const string Number = "global::Number";
    
    public static readonly SealClass Class = SealClass.CreateGlobal("Number", SealValueType.Number);

    public static void Generate(PrototypeAssembly pAssembly)
    {
        SealClassFactory.Generate(typeof(SealNumber), pAssembly, Class);
    }
    
    [ConstantExport] public const double Inf     = double.PositiveInfinity;
    [ConstantExport] public const double Epsilon = double.Epsilon;
    [ConstantExport] public const double Max     = double.MaxValue;
    [ConstantExport] public const double Min     = double.MinValue;
    [ConstantExport] public const double NaN     = double.NaN;
    
    [SealConstructor]
    [FunctionExport("Any")]
    public static SealValue _new(SealValue[] args)
    {
        SealValue value = args[0];

        return value.ValueType switch
        {
            SealValueType.Nil    => 0,
            SealValueType.Bool   => value.AsBool() ? 1 : 0,
            SealValueType.Number => value,
            SealValueType.String => 
                double.TryParse(value.AsString(), out double parsedValue)
                    ? parsedValue
                    : SealValue.Nil,
            _ => 0,
        };
    }

    [FunctionExport(SealString.String, MinArgs = 0)]
    public static SealValue to_string(SealValue self, SealValue[] args) => args.Length switch
    {
        0 => self.AsDouble().ToString(CultureInfo.InvariantCulture),
        1 => self.AsDouble().ToString(args[0].AsString()),
        _ => throw new ArgumentException($"Expected 0 or 1 args, got {args.Length}.")
    };
}