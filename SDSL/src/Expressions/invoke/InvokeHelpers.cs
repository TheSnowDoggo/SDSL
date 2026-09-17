namespace SDSL.Expressions;

public static class InvokeHelpers
{
	public static Variant[] EvaluateArgs(Variable[] variables, Expression[] argumentExpressions)
	{
		int length = argumentExpressions.Length;

		if (length == 0)
		{
			return [];
		}
        
		var args = new Variant[length];

		for (int i = 0; i < length; i++)
		{
			args[i] = argumentExpressions[i].Evaluate(variables);
		}

		return args;
	}
}