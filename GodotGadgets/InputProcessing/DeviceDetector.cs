using System.Runtime.CompilerServices;

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

    public void Handle(InputSample sample)
    {
        if (IsRandomStickDrift(sample)) return;

        var detected = Classify(sample);
        if (detected == Current) return;

        var previous = Current;
        Current = detected;
        DeviceChanged?.Invoke(previous, detected);
    }

    static bool IsRandomStickDrift(InputSample sample) =>
        sample is InputSample.GamepadAxis { Value: var value } && MathF.Abs(value) < JoystickMotionDetectDeadzone;

    static InputDevice Classify(InputSample sample) => sample switch
    {
        InputSample.Mouse => Mouse.Instance,
        InputSample.Keyboard => Keyboard.Instance,
        InputSample.GamepadButton { Gamepad: var gamepad } => gamepad,
        InputSample.GamepadAxis { Gamepad: var gamepad } => gamepad,
        _ => throw new SwitchExpressionException(),
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

    public static Gamepad? Find(int deviceId)
    {
        if (deviceId < 0 || !Input.IsJoyKnown(deviceId))
            return null;

        if (BrandCache.TryGetValue(deviceId, out var brand))
            return brand.ToGamepad();

        brand = Input.GetJoyName(deviceId).ToGamepadBrand();
        BrandCache[deviceId] = brand;

        return brand.ToGamepad();
    }

    extension(GamepadBrand brand)
    {
        Gamepad ToGamepad() => brand switch
        {
            GamepadBrand.Xbox => Xbox,
            GamepadBrand.PlayStation => PlayStation,
            GamepadBrand.Nintendo => Nintendo,
            _ => Xbox,
        };
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