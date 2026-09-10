using SDSL.Prototypes;

namespace SDSL.Factory;

public static class SealEnumFactory
{
    public static void Generate(PrototypeAssembly pAssembly, SealClass sClass, Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new NativeFactoryException($"Expected enum type, got {enumType}.");
        }

        PrototypeClass pClass = pAssembly.CreateClass(sClass);

        string[] names = enumType.GetEnumNames();
        Array values = enumType.GetEnumValues();

        int length = names.Length;

        for (int i = 0; i < length; i++)
        {
            string name = names[i];

            if (name == "Names")
            {
                throw new NativeFactoryException($"Enum with name '{name}' is unsupported, name is reserved.");
            }
            
            double value = Convert.ToDouble(values.GetValue(i));

            var pConstant = new PrototypeConstant(
                SourceLocation.Native,
                name,
                value
            );
            
            pClass.NativeConstants.Add(pConstant);
        }
    }
}