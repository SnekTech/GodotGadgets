namespace GodotGadgets.InputProcessing;

public sealed class DeviceDetector
{
    public InputDevice Current { get; private set; } = Unknown.Instance;
    public event DeviceChangedInputHandler? DeviceChanged;
    
    // todo: handle controller plugin or pullout

    public void Process(InputEvent @event)
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

    static InputDevice Classify(InputEvent @event) => @event switch
    {
        InputEventKey or InputEventMouse => Keyboard.Instance,
        InputEventJoypadButton joypadButton => new Gamepad(joypadButton.Device.ToGamepadBrand()),
        InputEventJoypadMotion joypadMotion => new Gamepad(joypadMotion.Device.ToGamepadBrand()),
        _ => Unknown.Instance,
    };
}

public delegate void DeviceChangedInputHandler(InputDevice previous, InputDevice detected);

file static class DeviceExtension
{
    static readonly HashSet<string> PlayStationKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "PS4", "PS5", "PlayStation",
    };
    static readonly HashSet<string> NintendoKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "Nintendo", "Switch",
    };

    extension(int deviceId)
    {
        internal GamepadBrand ToGamepadBrand()
        {
            var name = Input.GetJoyName(deviceId);

            return name switch
            {
                _ when name.ContainsAnyKeywordFrom(PlayStationKeywords)
                    => GamepadBrand.PlayStation,

                _ when name.ContainsAnyKeywordFrom(NintendoKeywords)
                    => GamepadBrand.Nintendo,

                _ => GamepadBrand.Xbox, // XInput / generic → default to Xbox
            };
        }
    }

    extension(string deviceName)
    {
        bool ContainsAnyKeywordFrom(HashSet<string> keywords) =>
            keywords.Any(keyword => deviceName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
}