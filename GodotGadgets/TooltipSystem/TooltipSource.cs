using GodotGadgets.Extensions;

namespace GodotGadgets.TooltipSystem;

public abstract record TooltipSource
{
    public abstract Rect2 GlobalRect { get; }

    public IDisposable Subscribe(Action onPointerEntered, Action onPointerExited)
    {
        AddHoverHandlers(onPointerEntered, onPointerExited);
        return new ActionDisposable(() => RemoveHoverHandlers(onPointerEntered, onPointerExited));
    }

    protected abstract void AddHoverHandlers(Action onPointerEntered, Action onPointerExited);
    protected abstract void RemoveHoverHandlers(Action onPointerEntered, Action onPointerExited);

    public sealed record FromControl(Control Target) : TooltipSource
    {
        public override Rect2 GlobalRect => Target.GetGlobalRect();

        protected override void AddHoverHandlers(Action onPointerEntered, Action onPointerExited)
        {
            Target.MouseEntered += onPointerEntered;
            Target.MouseExited += onPointerExited;
        }

        protected override void RemoveHoverHandlers(Action onPointerEntered, Action onPointerExited)
        {
            Target.MouseEntered -= onPointerEntered;
            Target.MouseExited -= onPointerExited;
        }
    }

    public sealed record FromArea(Area2D Target) : TooltipSource
    {
        /// <summary>
        /// 期望 Area2D 之下有且仅有一个矩形碰撞形状，取其全局包围盒；
        /// 其余情况退化成 target 位置上的一个零尺寸矩形。
        /// </summary>
        public override Rect2 GlobalRect => Target.GetChildrenOfType<CollisionShape2D>().ToList() switch
        {
            [{ Shape: RectangleShape2D rectangle } collider] => collider.GlobalTransform * rectangle.GetRect(),
            _ => new Rect2(Target.GlobalPosition, Vector2.Zero),
        };

        protected override void AddHoverHandlers(Action onPointerEntered, Action onPointerExited)
        {
            Target.MouseEntered += onPointerEntered;
            Target.MouseExited += onPointerExited;
        }

        protected override void RemoveHoverHandlers(Action onPointerEntered, Action onPointerExited)
        {
            Target.MouseEntered -= onPointerEntered;
            Target.MouseExited -= onPointerExited;
        }
    }
}

public sealed class ActionDisposable(Action onDispose) : IDisposable
{
    public void Dispose() => onDispose();
}