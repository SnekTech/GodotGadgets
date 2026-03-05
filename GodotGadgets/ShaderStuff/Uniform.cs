using GTweens.Extensions;
using GTweens.Tweeners;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace GodotGadgets.ShaderStuff;

[UsedImplicitly]
public class Uniform<[MustBeVariant] T>(ShaderMaterial shaderMaterial, StringName name) where T : struct
{
    [UsedImplicitly]
    public T Value
    {
        get;
        set
        {
            field = value;
            shaderMaterial.SetShaderParameter(name, Variant.From(value));
        }
    }
}

public static class UniformExtensions
{
    extension(ShaderMaterial shaderMaterial)
    {
        [UsedImplicitly]
        public Uniform<T> GetUniform<[MustBeVariant] T>(StringName name) where T : struct =>
            new(shaderMaterial, name);
    }

    extension<[MustBeVariant] T>(Uniform<T> uniform) where T : struct
    {
        Tweener<T>.Getter Getter => () => uniform.Value;
        Tweener<T>.Setter Setter => v => uniform.Value = v;
    }

    extension(Uniform<float> uniform)
    {
        [UsedImplicitly]
        public GTween Tween(float to, float duration)
            => GTweenExtensions.Tween(uniform.Getter, uniform.Setter, to, duration);
    }

    extension(Uniform<Vector2> uniform)
    {
        [UsedImplicitly]
        public GTween Tween(Vector2 to, float duration) =>
            GTweenGodotExtensions.Tween(uniform.Getter, uniform.Setter, to, duration);
    }

    extension(Uniform<Color> uniform)
    {
        [UsedImplicitly]
        public GTween Tween(Color to, float duration) =>
            GTweenGodotExtensions.Tween(uniform.Getter, uniform.Setter, to, duration);
    }
}