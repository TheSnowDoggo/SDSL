namespace SDSL.Native;

public static class GlobalEnums
{
    public static NativeClass KeyCodeClass { get; } = NativeClass.InheritObject("KeyCode");
    public static NativeClass ConsoleColorClass { get; } = NativeClass.InheritObject("ConsoleColor");
}