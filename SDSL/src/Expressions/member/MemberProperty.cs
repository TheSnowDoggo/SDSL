namespace SDSL.Expressions;

public abstract class MemberProperty
{
	public abstract SealValue Get(SealObject self);

	public abstract void Set(SealObject self, SealValue value);
}