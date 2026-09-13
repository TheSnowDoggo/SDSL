namespace SDSL;

[ClassExport]
public static class ObjectClass
{
	public static NativeVariantClass Class { get; } = new NativeVariantClass("Object", VariantType.Nil);
	
	public static void Generate(VariantAssembly variantAssembly)
	{
		VariantClassFactory.GenerateClass(variantAssembly, typeof(ObjectClass), Class);
	}

	[FunctionExport]
	public static Variant to_string(Variant self)
	{
		return self.ToString();
	}
	
	[FunctionInfo("other")]
	[FunctionExport("Any")]
	public static Variant equals(Variant self, Variant[] args)
	{
		return self.Equals(args[0]);
	}
	
	[FunctionExport]
	public static Variant get_hash_code(Variant self)
	{
		return self.GetHashCode();
	}
	
	[FunctionExport]
	public static Variant get_type(Variant self)
	{
		return self.Class;
	}
}