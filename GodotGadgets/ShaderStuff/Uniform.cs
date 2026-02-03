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
        public Uniform<T> GetUniform<[MustBeVariant] T>(StringName name) where T : struct =>
            new(shaderMaterial, name);
    }
}