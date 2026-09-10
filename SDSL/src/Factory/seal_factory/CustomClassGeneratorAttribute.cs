namespace SDSL.Factory;

[AttributeUsage(AttributeTargets.Class)]
public class CustomClassGeneratorAttribute : Attribute
{
	public string GenerateMethod { get; init; }
}