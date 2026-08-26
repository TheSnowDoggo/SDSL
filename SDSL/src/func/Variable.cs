namespace SDSL;

public struct Variable
{
    public Variable(SealClass @class, bool isConst, SealValue defaultValue = default)
    {
        Class = @class;
        IsConst = isConst;
        Value = defaultValue;
    }

    public readonly SealClass Class;
    public readonly bool IsConst;
    public SealValue Value;
}