using JetBrains.Annotations;

namespace GodotGadgets.ShaderStuff;

[UsedImplicitly]
public class Uniform<[MustBeVariant] T>(ShaderMaterial shaderMaterial, StringName name) where T : struct
{
    private T _value;

    [UsedImplicitly]
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            shaderMaterial.SetShaderParameter(name, Variant.From(value));
        }
    }
}