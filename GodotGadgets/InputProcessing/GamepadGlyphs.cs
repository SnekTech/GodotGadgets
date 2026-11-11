namespace GodotGadgets.InputProcessing;

public interface IGamepadGlyphs
{
    Texture2D Glyph(GamepadFaceButton button);
}

public enum GamepadFaceButton
{
    West,
    North,
    East,
    South,
}

public readonly record struct InputHint(Texture2D Glyph, string Label);