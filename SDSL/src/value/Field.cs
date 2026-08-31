namespace SDSL;

public struct Field
{
    public Field(
        SealClass sClass,
        bool isConst,
        SealValue defaultValue)
    {
        Class = sClass;
        IsConst = isConst;
        Value = defaultValue;
    }

    public readonly SealClass Class;
    public bool IsConst;
    public SealValue Value;
}