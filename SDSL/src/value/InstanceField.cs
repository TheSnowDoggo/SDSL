using SDSL.Expressions;

namespace SDSL;

public readonly struct InstanceField
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

    public readonly SealClass Class;
    public readonly bool IsConst;
    public readonly Expression Expression;
}