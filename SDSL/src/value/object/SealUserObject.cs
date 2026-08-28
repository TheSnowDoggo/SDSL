namespace SDSL;

public class SealUserObject : SealObject
{
    public SealUserObject(SealClass sClass, Variable[] fields)
    {
        TypeClass = sClass;
        Fields = fields;
    }
    
    public override SealClass TypeClass { get; }
    public Variable[] Fields { get; }
}