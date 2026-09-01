namespace GodotGadgets.UI.ScrollMenuCore;

/// <summary>
/// Translates raw Godot input into ScrollMenuInput intents.
/// Stateless mechanical translation — no menu decisions. Hit-testing is injected.
/// </summary>
public sealed class ScrollMenuInputMapper(Func<Vector2, IScrollMenuItem?> hitTest, float stickDeadZone = 0.5f)
{
    public ScrollMenuInput? Map(InputEvent @event) => @event switch
    {
        // mouse wheel
        InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.WheelUp } => new NavigateInput(NavigateDirection.Up),
        InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.WheelDown } => new NavigateInput(NavigateDirection.Down),
        // left click → click intent (domain decides focus vs confirm)
        InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouse => MapClick(mouse),
        // keyboard: arrows + enter (edge-triggered, ignore echo)
        InputEventKey { Pressed: true, Echo: false, Keycode: Key.Up } => new NavigateInput(NavigateDirection.Up),
        InputEventKey { Pressed: true, Echo: false, Keycode: Key.Down } => new NavigateInput(NavigateDirection.Down),
        InputEventKey { Pressed: true, Echo: false, Keycode: Key.Enter or Key.KpEnter } => new ConfirmInput(),
        // gamepad: d-pad + A (JoyButton.A maps by physical position, works across brands)
        InputEventJoypadButton { Pressed: true, ButtonIndex: JoyButton.DpadUp } => new NavigateInput(NavigateDirection.Up),
        InputEventJoypadButton { Pressed: true, ButtonIndex: JoyButton.DpadDown } => new NavigateInput(NavigateDirection.Down),
        InputEventJoypadButton { Pressed: true, ButtonIndex: JoyButton.A } => new ConfirmInput(),
        // left stick Y axis (positive = down), deadzone applied
        InputEventJoypadMotion { Axis: JoyAxis.LeftY, AxisValue: var v } => MapStick(v),
        _ => null,
    };

    ScrollMenuInput? MapClick(InputEventMouseButton mouse) =>
        hitTest(mouse.Position) is { } item ? new ClickInput(item) : null;

    ScrollMenuInput? MapStick(float axisValue) => axisValue switch
    {
        var v when v < -stickDeadZone => new NavigateInput(NavigateDirection.Up),
        var v when v > +stickDeadZone => new NavigateInput(NavigateDirection.Down),
        _ => null,
    };
}
