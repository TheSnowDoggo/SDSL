using SDSL.Expressions;

namespace SDSL;

public readonly struct UserFieldInfo
{
	public UserFieldInfo(VariantClass fieldClass, Expression expression)
	{
		FieldClass = fieldClass;
		Expression = expression;
	}
	
	public VariantClass FieldClass { get; }
	public Expression Expression { get; }
}