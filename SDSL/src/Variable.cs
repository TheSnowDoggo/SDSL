namespace SDSL;

public struct Variable
{
	public Variable(
        SealClass sClass,
        SealValue defaultValue)
    {
        Class = sClass;
        Value = defaultValue;
    }

    public readonly SealClass Class;
    public SealValue Value;
}