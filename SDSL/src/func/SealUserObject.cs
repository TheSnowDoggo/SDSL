namespace SDSL;

public class SealUserObject : SealObject
{
    public SealUserObject(SealClass @class, Variable[] fields)
    {
        Class = @class;
        Fields = fields;
    }
    
    public override SealClass Class { get; }
    public Variable[] Fields { get; }
}