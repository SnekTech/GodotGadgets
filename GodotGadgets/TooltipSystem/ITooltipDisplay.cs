using GodotGadgets.Extensions;

namespace GodotGadgets.TooltipSystem;

public interface ITooltipDisplay
{
    void ShowTooltip(TooltipContent content, Rect2 targetGlobalRect);
    void HideTooltip();
}

public sealed class TooltipDisplayConsole : ITooltipDisplay
{
    public void ShowTooltip(TooltipContent content, Rect2 targetGlobalRect)
    {
        "Showing tooltip: \n".DumpGd();
        content.DumpGd(nameof(content));
        targetGlobalRect.DumpGd(nameof(targetGlobalRect));
    }

    public void HideTooltip()
    {
        "Hiding tooltip: \n".DumpGd();
    }
}