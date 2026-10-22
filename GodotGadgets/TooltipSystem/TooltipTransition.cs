namespace GodotGadgets.TooltipSystem;

public abstract record TooltipTransition
{
    public sealed record Unchanged : TooltipTransition;
    public sealed record Show(TooltipView View) : TooltipTransition;
    public sealed record Replace(TooltipView View) : TooltipTransition;
    public sealed record Hide : TooltipTransition;

    public static TooltipTransition From(TooltipView? previous, TooltipView? next) => (previous, next) switch
    {
        (null, null) => new Unchanged(),
        (null, { } view) => new Show(view),
        (not null, null) => new Hide(),
        ({} current, { } view) when current == view => new Unchanged(),
        (not null, { } view) => new Replace(view),
    };
}