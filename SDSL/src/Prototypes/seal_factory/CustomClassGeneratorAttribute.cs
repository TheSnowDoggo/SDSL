namespace SDSL.Prototypes;

[AttributeUsage(AttributeTargets.Class)]
public class CustomClassGeneratorAttribute : Attribute
{
	public string GenerateMethod { get; init; }
}