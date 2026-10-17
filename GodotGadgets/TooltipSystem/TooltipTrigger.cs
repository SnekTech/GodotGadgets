namespace GodotGadgets.TooltipSystem;

public sealed class TooltipTrigger(TooltipSource source, TooltipContent content, ITooltipDisplay display)
    : IDisposable
{
    readonly IDisposable _subscription = source.Subscribe(
        onPointerEntered: () => display.PointerEntered(content, source.GlobalRect),
        onPointerExited: display.PointerExited
    );

    public static TooltipTrigger Attach(TooltipSource source, TooltipContent content, ITooltipDisplay display)
        => new(source, content, display);

    public void Dispose()
    {
        // todo: 处理 dispose 幂等问题
        display.PointerExited();
        _subscription.Dispose();
    }
}