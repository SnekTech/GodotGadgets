namespace GodotGadgets.TooltipSystem;

public readonly record struct RequestId(long Value)
{
    public RequestId GetNext() => new(Value + 1);
}

public readonly record struct TooltipRequest(RequestId Id, TooltipContent Content, Rect2 TargetRect);

public abstract record TooltipSession
{
    public sealed record Hidden : TooltipSession;
    public sealed record WaitingForHover(TooltipRequest Request) : TooltipSession;
    public sealed record Visible(TooltipRequest Request) : TooltipSession;

    public abstract record Event
    {
        public sealed record PointerEntered(TooltipRequest Request) : Event;
        public sealed record PointerExited : Event;
        public sealed record HoverDelayElapsed(RequestId Id) : Event;
    }

    public abstract record Effect
    {
        public sealed record StartHoverDelay(TimeSpan Delay, RequestId RequestId) : Effect;
        public sealed record CancelHoverDelay : Effect;
        public sealed record Show(TooltipRequest Request) : Effect;
        public sealed record FadeOut : Effect;
    }
}

public readonly record struct TooltipSessionWithEffects(
    TooltipSession Session,
    IReadOnlyList<TooltipSession.Effect> Effects);

public static class TooltipSessionMachine
{
    extension(TooltipSession session)
    {
        public TooltipSessionWithEffects Update(TooltipSession.Event evt, TimeSpan hoverDelay) =>
            (session, evt) switch
            {
                (TooltipSession.Hidden, TooltipSession.Event.PointerEntered { Request: var request })
                    => Delay(request, hoverDelay),

                // 立刻切换到另一个目标
                (TooltipSession.Visible, TooltipSession.Event.PointerEntered { Request: var request })
                    => Show(request),
                
                // 等待途中重新计时
                (TooltipSession.WaitingForHover, TooltipSession.Event.PointerEntered {Request:var request})
                    => Delay(request, hoverDelay),

                // 等待中途离开
                (TooltipSession.WaitingForHover, TooltipSession.Event.PointerExited)
                    => new TooltipSessionWithEffects(new TooltipSession.Hidden(),
                        [new TooltipSession.Effect.CancelHoverDelay()]),

                (TooltipSession.WaitingForHover { Request: var pendingRequest }, TooltipSession.Event.HoverDelayElapsed
                    {
                        Id: var elapsedId
                    })
                    when pendingRequest.Id == elapsedId
                    => Show(pendingRequest),

                (TooltipSession.Visible, TooltipSession.Event.PointerExited)
                    => new TooltipSessionWithEffects(new TooltipSession.Hidden(),
                        [new TooltipSession.Effect.FadeOut()]),


                // invalid event for current session, ignore
                _ => new TooltipSessionWithEffects(session, []),
            };
    }

    static TooltipSessionWithEffects Delay(TooltipRequest request, TimeSpan hoverDelay) =>
        hoverDelay > TimeSpan.Zero
            ? new TooltipSessionWithEffects(new TooltipSession.WaitingForHover(request),
                [new TooltipSession.Effect.CancelHoverDelay(), new TooltipSession.Effect.StartHoverDelay(hoverDelay, request.Id)])
            : Show(request);

    static TooltipSessionWithEffects Show(TooltipRequest request) =>
        new(new TooltipSession.Visible(request), [new TooltipSession.Effect.Show(request)]);
}