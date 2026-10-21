namespace GodotGadgets.TooltipSystem;

public sealed record TooltipConfig(TimeSpan HoverDelay, float Margin)
{
    public static TooltipConfig Default { get; } = new(TimeSpan.FromMilliseconds(350), 10);
}