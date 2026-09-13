namespace SDSL;

public class VariantAssemblyGenerator
{
	private readonly VariantAssembly _assembly;

	public VariantAssemblyGenerator(VariantAssembly assembly)
	{
		_assembly = assembly;
	}

	public void GenerateMembers()
	{
		foreach (UserVariantClass variantClass in _assembly.UserClasses)
		{
			GenerateFunctions(variantClass);
			
			GenerateProperties(variantClass);
		}	
	}

	private void GenerateFunctions(UserVariantClass variantClass)
	{
		
	}
	
	private void GenerateProperties(UserVariantClass variantClass)
	{
		
	}
}