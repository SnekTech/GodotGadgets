namespace GodotGadgets.InputProcessing;

public abstract record InputDevice;

public sealed record Keyboard : InputDevice
{
    public static Keyboard Instance { get; } = new();
}

public sealed record Gamepad(GamepadBrand Brand) : InputDevice;

public sealed record Unknown : InputDevice
{
    public static Unknown Instance { get; } = new();
}

public enum GamepadBrand
{
    Xbox,
    PlayStation,
    Nintendo,
}