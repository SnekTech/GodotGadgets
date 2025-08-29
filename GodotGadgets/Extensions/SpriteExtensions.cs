namespace GodotGadgets.Extensions;

public static class SpriteExtensions
{
    public static T GetMaterialAs<T>(this Sprite2D sprite) where T : Material
        => (T)sprite.Material;

    public static T GetTextureAs<T>(this Sprite2D sprite) where T : Texture2D
        => (T)sprite.Texture;
    
    public static void SetSize(this GradientTexture2D gradientTexture2D, Vector2 size)
    {
        var (width, height) = size.ToVector2I();
        gradientTexture2D.SetWidth(width);
        gradientTexture2D.SetHeight(height);
    }
}