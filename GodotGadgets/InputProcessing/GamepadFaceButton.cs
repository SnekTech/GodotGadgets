namespace GodotGadgets.InputProcessing;

public partial union GamepadFaceButton(GamepadFaceButton.West, GamepadFaceButton.North, GamepadFaceButton.East, GamepadFaceButton.South);

public partial union GamepadFaceButton
{
    public readonly record struct West;
    public readonly record struct North;
    public readonly record struct East;
    public readonly record struct South;
}
