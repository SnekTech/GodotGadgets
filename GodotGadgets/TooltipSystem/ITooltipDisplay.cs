namespace GodotGadgets.TooltipSystem;

public interface ITooltipDisplay
{
    void PointerEntered(TooltipContent content, Rect2 targetGlobalRect);
    void PointerExited();
}