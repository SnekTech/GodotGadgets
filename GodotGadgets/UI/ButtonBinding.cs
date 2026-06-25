namespace GodotGadgets.UI;

public readonly record struct ButtonAndHandler(Button Button, Action Handler);

public sealed class ButtonBindings : IDisposable
{
    readonly ButtonAndHandler[] _bindings;

    public ButtonBindings(params ButtonAndHandler[] bindings)
    {
        _bindings = bindings.ToArray();

        foreach (var binding in _bindings)
        {
            binding.Button.Pressed += binding.Handler;
        }
    }

    public void Dispose()
    {
        foreach (var binding in _bindings)
        {
            binding.Button.Pressed -= binding.Handler;
        }
    }
}

public static class ButtonHandlerExtensions
{
    extension(Button button)
    {
        public ButtonAndHandler BindHandler(Action handler) => new(button, handler);
    }
}