namespace GodotGadgets.InputProcessing;

public abstract record InputDevice
{
    public sealed record Mouse : InputDevice
    {
        public static Mouse Instance { get; } = new();
    }

    public sealed record Keyboard : InputDevice
    {
        public static Keyboard Instance { get; } = new();
    }

    public sealed record Gamepad(GamepadBrand Brand) : InputDevice;

    public sealed record NoDevice : InputDevice
    {
        public static NoDevice Instance { get; } = new();
    }
}