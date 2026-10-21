using System.Runtime.CompilerServices;

namespace GodotGadgets.TooltipSystem;

public readonly record struct HoverKey(int Value)
{
    public HoverKey GetNext() => new(Value + 1);
}

public readonly record struct TooltipView(HoverKey Key, TooltipContent Content, Rect2 Anchor);
public sealed record HoverRequest(TooltipView View, TimeSpan StartedAt);

public abstract record TooltipState
{
    public sealed record Hidden : TooltipState
    {
        public static readonly Hidden Instance = new();
    }

    public sealed record Waiting(HoverRequest Request) : TooltipState;
    public sealed record Shown(HoverRequest Request) : TooltipState;

    public abstract record Input
    {
        public sealed record Entered(TooltipView View) : Input;
        public sealed record Exited(HoverKey Key) : Input;

        public sealed record Tick : Input
        {
            public static readonly Tick Instance = new();
        }
    }
}

public readonly record struct TooltipUpdate(TooltipState State, TooltipView? Presentation);

public static class TooltipStateMachine
{
    extension(TooltipState state)
    {
        public static TooltipState Initial => TooltipState.Hidden.Instance;

        public TooltipUpdate Update(TooltipConfig config, TimeSpan now, TooltipState.Input input) =>
            (state, input) switch
            {
                (TooltipState.Hidden, TooltipState.Input.Entered entered)
                    => BeginHover(config, now, entered),
                (TooltipState.Hidden, _) => Stay(state),

                (TooltipState.Waiting, TooltipState.Input.Entered entered)
                    => BeginHover(config, now, entered),
                (TooltipState.Waiting waiting, TooltipState.Input.Exited exited)
                    when exited.Key == waiting.Request.View.Key
                    => new TooltipUpdate(TooltipState.Hidden.Instance, null),
                (TooltipState.Waiting, TooltipState.Input.Exited) => Stay(state),
                (TooltipState.Waiting waiting, TooltipState.Input.Tick)
                    when now - waiting.Request.StartedAt >= config.HoverDelay
                    => Show(waiting.Request),
                (TooltipState.Waiting, TooltipState.Input.Tick) => Stay(state),

                (TooltipState.Shown, TooltipState.Input.Entered entered)
                    => Show(new HoverRequest(entered.View, now)),
                (TooltipState.Shown shown, TooltipState.Input.Exited exited)
                    when exited.Key == shown.Request.View.Key
                    => new TooltipUpdate(TooltipState.Hidden.Instance, null),
                (TooltipState.Shown shown, TooltipState.Input.Exited) => Keep(shown),
                (TooltipState.Shown shown, TooltipState.Input.Tick) => Keep(shown),

                _ => throw new SwitchExpressionException(),
            };

        public bool NeedsTick => state is TooltipState.Waiting;
    }

    static TooltipUpdate BeginHover(TooltipConfig config, TimeSpan now, TooltipState.Input.Entered entered)
    {
        var request = new HoverRequest(entered.View, now);
        return config.HoverDelay <= TimeSpan.Zero
            ? Show(request)
            : new TooltipUpdate(new TooltipState.Waiting(request), null);
    }

    static TooltipUpdate Show(HoverRequest request) =>
        new(new TooltipState.Shown(request),request.View);

    static TooltipUpdate Keep(TooltipState.Shown shown) =>
        new(shown, shown.Request.View);

    static TooltipUpdate Stay(TooltipState state) => new(state, null);
}