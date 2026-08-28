namespace SDSL;

public class SealUserObject : SealObject
{
    public SealUserObject(SealClass sClass, Variable[] fields)
    {
        Class = sClass;
        Fields = fields;
    }
    
    public override SealClass Class { get; }
    public Variable[] Fields { get; }
}