namespace GodotGadgets.TooltipSystem;

public static class TooltipPlacement
{
    /// <summary>
    /// 由 <see cref="TooltipSide.From"/> 决定方向，再把结果钳进视口。
    /// </summary>
    public static Vector2 Resolve(Rect2 targetRect, Vector2 tooltipSize, Vector2 viewportSize, float margin)
    {
        var topLeft = TooltipSide.From(targetRect, tooltipSize, viewportSize, margin)
            .TopLeft(targetRect, tooltipSize, margin);

        return new Vector2(
            float.Clamp(topLeft.X, 0f, float.Max(0f, viewportSize.X - tooltipSize.X)),
            float.Clamp(topLeft.Y, 0f, float.Max(0f, viewportSize.Y - tooltipSize.Y)));
    }
}
