namespace SDSL.Native;

public static class BoolClass
{
	public static NativeClass Class { get; } = NativeClass.CreatePrimitive("Bool", VariantType.Bool);
}