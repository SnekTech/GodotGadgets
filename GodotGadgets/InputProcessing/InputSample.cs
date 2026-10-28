namespace GodotGadgets.InputProcessing;

public abstract record InputSample
{
    public sealed record Mouse : InputSample
    {
        public static Mouse Instance { get; } = new();
    }

    public sealed record Keyboard : InputSample
    {
        public static Keyboard Instance { get; } = new();
    }

    public sealed record GamepadButton(InputDevice.Gamepad Gamepad) : InputSample;
    public sealed record GamepadAxis(InputDevice.Gamepad Gamepad, float Value) : InputSample;
}