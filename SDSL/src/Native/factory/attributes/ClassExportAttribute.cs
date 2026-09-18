namespace SDSL;

[AttributeUsage(AttributeTargets.Class)]
public class ClassExportAttribute : Attribute
{
    public string GenerateMethodName { get; init; }
}