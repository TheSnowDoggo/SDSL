using System.Diagnostics.CodeAnalysis;

namespace SDSL.Expressions;

public class NativeProperty : MemberProperty
{
	private readonly Func<SealObject, SealValue> _getter;
	private readonly Action<SealObject, SealValue> _setter;
	
	public NativeProperty(
		Func<SealObject, SealValue> getter,
		[AllowNull] Action<SealObject, SealValue> setter)
	{
		_getter = getter;
		_setter = setter;
	}

	public override SealValue Get(SealObject self)
	{
		return _getter(self);
	}

	public override void Set(SealObject self, SealValue value)
	{
		if (_setter == null)
		{
			throw new InvalidOperationException("Field is const.");
		}

		_setter(self, value);
	}
}