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
        public override Rect2 GlobalRect =>
            Target.CollisionShape.GetShape().GetRect() with { Position = Target.GlobalPosition };

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