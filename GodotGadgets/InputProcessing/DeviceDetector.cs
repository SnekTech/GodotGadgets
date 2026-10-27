namespace GodotGadgets.InputProcessing;

/// <summary>
/// The instance needs some node (autoload) to call <see cref="Handle"/> to function properly.
/// </summary>
public sealed class DeviceDetector
{
    const float JoystickMotionDetectDeadzone = 0.2f;

    public InputDevice Current { get; private set; } = Unknown.Instance;
    public event DeviceChangedInputHandler? DeviceChanged;

    // todo: handle controller plugin or pullout

    public void Handle(InputEvent @event)
    {
        var detected = Classify(@event);
        if (detected is Unknown || detected == Current)
        {
            return;
        }

        var previous = Current;
        Current = detected;
        DeviceChanged?.Invoke(previous, detected);
    }

    static InputDevice Classify(InputEvent evt) => evt switch
    {
        InputEventMouse => Mouse.Instance,
        InputEventKey => Keyboard.Instance,
        InputEventJoypadButton { Device: var deviceId } => deviceId.ToGamepad(),
        InputEventJoypadMotion
            {
                Axis: JoyAxis.LeftX or JoyAxis.LeftY or JoyAxis.RightX or JoyAxis.RightY,
                Device: var deviceId, AxisValue: var axisValue,
            }
            when MathF.Abs(axisValue) > JoystickMotionDetectDeadzone => deviceId.ToGamepad(),
        _ => Unknown.Instance,
    };
}

public delegate void DeviceChangedInputHandler(InputDevice previous, InputDevice detected);

public static class Gamepads
{
    static readonly HashSet<string> PlayStationKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "PS4", "PS5", "PlayStation",
    };
    static readonly HashSet<string> NintendoKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "Nintendo", "Switch",
    };

    static readonly Gamepad Xbox = new(GamepadBrand.Xbox);
    static readonly Gamepad PlayStation = new(GamepadBrand.PlayStation);
    static readonly Gamepad Nintendo = new(GamepadBrand.Nintendo);

    static readonly Dictionary<int, GamepadBrand> BrandCache = [];

    public static void ForgetCache(int deviceId) => BrandCache.Remove(deviceId);

    extension(int deviceId)
    {
        internal Gamepad ToGamepad()
        {
            if (deviceId < 0 || !Input.IsJoyKnown(deviceId)) return Xbox;

            if (!BrandCache.TryGetValue(deviceId, out var brand))
            {
                brand = Input.GetJoyName(deviceId).ToGamepadBrand();
                BrandCache[deviceId] = brand;
            }

            return brand switch
            {
                GamepadBrand.Xbox => Xbox,
                GamepadBrand.PlayStation => PlayStation,
                GamepadBrand.Nintendo => Nintendo,
                _ => Xbox,
            };
        }
    }

    extension(string deviceName)
    {
        GamepadBrand ToGamepadBrand() => deviceName switch
        {
            _ when deviceName.ContainsAnyKeywordFrom(PlayStationKeywords) => GamepadBrand.PlayStation,
            _ when deviceName.ContainsAnyKeywordFrom(NintendoKeywords) => GamepadBrand.Nintendo,
            _ => GamepadBrand.Xbox,
        };
        
        bool ContainsAnyKeywordFrom(HashSet<string> keywords) =>
            keywords.Any(keyword => deviceName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
}