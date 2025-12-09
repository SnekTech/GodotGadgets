namespace GodotGadgets.TooltipSystem;

public class TooltipTriggerBehavior(ITooltipTarget target)
{
    public TooltipContent Content { get; set; } = TooltipContent.New("default tooltip", "default content");
    public ITooltipDisplay TooltipDisplay { private get; set; } = new TooltipDisplayConsole();

    public void OnMouseEntered()
    {
        TooltipDisplay.ShowTooltip(Content, target.GlobalRect);
    }

    public void OnMouseExited()
    {
        TooltipDisplay.HideTooltip();
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
        public static TooltipTriggerBehavior New(Node parentNode) =>
            parentNode switch
            {
                Control c => new TooltipTriggerBehavior(c.ToTooltipTarget()), 
                Area2D area2D => new TooltipTriggerBehavior(area2D.ToTooltipTarget()),
                _ => throw new ArgumentException($"node {parentNode} is not Control or Area2D"),
            };
    }
}