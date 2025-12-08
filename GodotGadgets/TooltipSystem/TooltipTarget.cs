using GodotGadgets.Extensions;

namespace GodotGadgets.TooltipSystem;

public interface ITooltipTarget
{
    Rect2 GlobalRect { get; }
}

public sealed class Area2DTooltipTarget(Area2D area2D) : ITooltipTarget
{
    public Rect2 GlobalRect
    {
        get => field with { Position = area2D.GlobalPosition };
    } = area2D.CollisionShape.GetShape().GetRect();
}

public sealed class ControlTooltipTarget(Control control) : ITooltipTarget
{
    public Rect2 GlobalRect => control.GetGlobalRect();
}