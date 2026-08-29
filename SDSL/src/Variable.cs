namespace SDSL;

public struct Variable
{
    public Variable(
        SealClass sClass,
        bool isConst,
        SealValue defaultValue = default)
    {
        Class = sClass;
        IsConst = isConst;
        Value = defaultValue;
    }

    public readonly SealClass Class;
    public bool IsConst;
    public SealValue Value;
}