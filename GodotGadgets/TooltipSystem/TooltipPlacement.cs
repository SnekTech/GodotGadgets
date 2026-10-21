namespace GodotGadgets.TooltipSystem;

// todo: add some tests
public static class TooltipPlacement
{
    /// <summary>水平优先右侧、放不下翻左侧；垂直优先贴目标顶部往下、放不下翻上方；最后一律钳进视口。</summary>
    public static Vector2 Resolve(Rect2 targetRect, Vector2 tooltipSize, Vector2 viewportSize, float margin)
    {
        var x = FitsOnTheRight() ? targetRect.End.X + margin : targetRect.Position.X - margin - tooltipSize.X;
        var y = FitsBelow() ? targetRect.Position.Y : targetRect.Position.Y - margin - tooltipSize.Y;

        return new Vector2(ClampX(x), ClampY(y));

        bool FitsOnTheRight() => targetRect.End.X + margin + tooltipSize.X <= viewportSize.X;
        bool FitsBelow() => targetRect.Position.Y + tooltipSize.Y <= viewportSize.Y;

        float ClampX(float value) => float.Clamp(value, 0f, float.Max(0f, viewportSize.X - tooltipSize.X));
        float ClampY(float value) => float.Clamp(value, 0f, float.Max(0f, viewportSize.Y - tooltipSize.Y));
    }
}