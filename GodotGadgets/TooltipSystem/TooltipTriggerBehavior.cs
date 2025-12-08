namespace GodotGadgets.TooltipSystem;

public class TooltipTriggerBehavior(ITooltipTarget target, ITooltipDisplay tooltipDisplay)
{
    public TooltipContent Content { get; set; } = TooltipContent.New("default tooltip", "default content");

    public void OnMouseEntered()
    {
        tooltipDisplay.ShowTooltip(Content, target.GlobalRect);
    }

    public void OnMouseExited()
    {
        tooltipDisplay.HideTooltip();
    }
}

public static class TooltipFactory
{
    extension(Area2D area2D)
    {
        Area2DTooltipTarget ToTooltipTarget() => new(area2D);
    }

    extension(Control control)
    {
        ControlTooltipTarget ToTooltipTarget() => new(control);
    }

    extension(TooltipTriggerBehavior)
    {
        public static TooltipTriggerBehavior FromArea2D(Area2D area2D, ITooltipDisplay display) =>
            new(area2D.ToTooltipTarget(), display);
        public static TooltipTriggerBehavior FromControl(Control control, ITooltipDisplay display) =>
            new(control.ToTooltipTarget(), display);
    }
}