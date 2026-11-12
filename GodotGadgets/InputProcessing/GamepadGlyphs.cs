namespace GodotGadgets.InputProcessing;

public interface IGamepadGlyphProvider
{
    Texture2D Glyph(GamepadFaceButton button);
}
