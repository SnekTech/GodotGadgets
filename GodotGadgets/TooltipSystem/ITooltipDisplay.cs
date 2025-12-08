namespace GodotGadgets.TooltipSystem;

public interface ITooltipDisplay
{
    void ShowTooltip(TooltipContent content, Rect2 targetGlobalRect);
    void HideTooltip();
}