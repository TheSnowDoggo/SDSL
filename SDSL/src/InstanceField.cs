using SDSL.Expressions;

namespace SDSL;

public class InstanceField
{
    public InstanceField(
        SealClass sClass,
        bool isConst,
        Expression expression)
    {
        Class = sClass;
        IsConst = isConst;
        Expression = expression;
    }
    
    public SealClass Class { get; }
    public bool IsConst { get; }
    public Expression Expression { get; }
}