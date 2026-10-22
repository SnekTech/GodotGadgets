namespace GodotGadgets.TooltipSystem;

/// <summary>
/// tooltip 相对 target 的摆放方向。<see cref="From"/> 是唯一的分支决策，
/// <see cref="TopLeft"/> 只把方向机械地翻译成坐标。
/// </summary>
public abstract record TooltipSide
{
    /// <summary>优先右侧、向下展开；放不下才翻转。</summary>
    public static TooltipSide From(Rect2 targetRect, Vector2 tooltipSize, Vector2 viewportSize, float margin)
    {
        var fitsOnTheRight = targetRect.End.X + margin + tooltipSize.X <= viewportSize.X;
        var fitsDownward = targetRect.Position.Y + tooltipSize.Y <= viewportSize.Y;

        return (fitsOnTheRight, fitsDownward) switch
        {
            (true, true) => RightDown.Instance,
            (true, false) => RightUp.Instance,
            (false, true) => LeftDown.Instance,
            (false, false) => LeftUp.Instance,
        };
    }

    /// <summary>该方向下 tooltip 的左上角，尚未钳进视口。</summary>
    public abstract Vector2 TopLeft(Rect2 targetRect, Vector2 tooltipSize, float margin);

    /// <summary>贴 target 右侧，顶边与 target 顶边齐平。</summary>
    public sealed record RightDown : TooltipSide
    {
        public static readonly RightDown Instance = new();

        public override Vector2 TopLeft(Rect2 targetRect, Vector2 tooltipSize, float margin) =>
            new(targetRect.End.X + margin, targetRect.Position.Y);
    }

    /// <summary>贴 target 右侧，底边在 target 顶边上方一个 margin。</summary>
    public sealed record RightUp : TooltipSide
    {
        public static readonly RightUp Instance = new();

        public override Vector2 TopLeft(Rect2 targetRect, Vector2 tooltipSize, float margin) =>
            new(targetRect.End.X + margin, targetRect.Position.Y - margin - tooltipSize.Y);
    }

    /// <summary>贴 target 左侧，顶边与 target 顶边齐平。</summary>
    public sealed record LeftDown : TooltipSide
    {
        public static readonly LeftDown Instance = new();

        public override Vector2 TopLeft(Rect2 targetRect, Vector2 tooltipSize, float margin) =>
            new(targetRect.Position.X - margin - tooltipSize.X, targetRect.Position.Y);
    }

    /// <summary>贴 target 左侧，底边在 target 顶边上方一个 margin。</summary>
    public sealed record LeftUp : TooltipSide
    {
        public static readonly LeftUp Instance = new();

        public override Vector2 TopLeft(Rect2 targetRect, Vector2 tooltipSize, float margin) =>
            new(targetRect.Position.X - margin - tooltipSize.X, targetRect.Position.Y - margin - tooltipSize.Y);
    }
}
