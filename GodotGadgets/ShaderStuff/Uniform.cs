using JetBrains.Annotations;

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