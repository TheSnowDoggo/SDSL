namespace SDSL;

public class SealObject
{
    public SealObject(SealClass @class, Variable[] fields)
    {
        Class = @class;
        Fields = fields;
    }
    
    public SealClass Class { get; }
    public Variable[] Fields { get; }
}