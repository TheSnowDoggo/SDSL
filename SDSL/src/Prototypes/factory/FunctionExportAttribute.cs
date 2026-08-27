namespace SDSL.Prototypes;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionExportAttribute : Attribute
{
    public FunctionExportAttribute(string signature)
    {
        Signature = signature;
    }
    
    public string Signature { get; }
}