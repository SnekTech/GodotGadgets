namespace GodotGadgets.TooltipSystem;

public interface ITooltipRenderer
{
    void Render(TooltipTransition transition);
}

public interface ITooltipAttacher
{
    IDisposable Attach(TooltipSource source, TooltipContent content);
}

public sealed class TooltipHost(TooltipConfig config, ITooltipRenderer renderer, Func<TimeSpan> clock)
    : ITooltipAttacher, IDisposable
{
    TooltipState _state = TooltipState.Initial;
    TooltipView? _rendered;
    HoverKey _nextKey;

    bool _disposed;

    public void Dispose() => _disposed = true;

    public IDisposable Attach(TooltipSource source, TooltipContent content)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _nextKey = _nextKey.GetNext();
        var key = _nextKey;

        var subscription = source.Subscribe(
            onPointerEntered: () =>
                Dispatch(new TooltipState.Input.Entered(new TooltipView(key, content, source.GlobalRect))),
            onPointerExited: () => Dispatch(new TooltipState.Input.Exited(key))
        );

        return new ActionDisposable(() =>
        {
            Dispatch(new TooltipState.Input.Exited(key));
            subscription.Dispose();
        });
    }

    public void Advance()
    {
        if (_disposed) return;

        if (_state.NeedsTick) Dispatch(TooltipState.Input.Tick.Instance);
    }

    void Dispatch(TooltipState.Input input)
    {
        if (_disposed) return;

        var (nextState, presentation) = _state.Update(config, clock(), input);
        _state = nextState;

        if (presentation == _rendered) return;

        var previous = _rendered;
        _rendered = presentation;

        renderer.Render(TooltipTransition.From(previous, presentation));
    }
}