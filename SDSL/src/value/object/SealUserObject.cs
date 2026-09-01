using SDSL.Classes;
using SDSL.Functions;

namespace SDSL;

public class SealUserObject : SealObject
{
    public SealUserObject(SealClass sClass, Field[] fields)
    {
        TypeClass = sClass;
        Fields = fields;
    }
    
    public override SealClass TypeClass { get; }
    public Field[] Fields { get; }

    public override string ToString()
    {
        if (SealAssembly.Current != null
            && TypeClass.FunctionTable != null
            && TypeClass.TryGetFunction("to_string", out Function function)
            && function.MinArgs == 0)
        {
            return function.MemberInvoke(this).ToString();
        }
        
        return base.ToString();
    }

    public override bool Equals(SealObject other)
    {
        if (SealAssembly.Current != null
            && TypeClass.FunctionTable != null
            && TypeClass.TryGetFunction("equals", out Function function)
            && function.MinArgs == 1
            && function.Args[0].Class == null
            && function.ReturnType == SealBool.Class)
        {
            return function.MemberInvoke(this, other).AsBool();
        }

        return base.Equals(other);
    }

    public override bool ToBool()
    {
        if (SealAssembly.Current != null
            && TypeClass.FunctionTable != null
            && TypeClass.TryGetFunction("to_bool", out Function function)
            && function.MinArgs == 0)
        {
            return function.MemberInvoke(this).ToBool();
        }
        
        return base.ToBool();
    }
}