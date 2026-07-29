using GodotGadgets.TweenStuff;
using GodotTask;
using GTweensGodot.Extensions;

namespace GodotGadgets.UI.ScrollMenuCore;

public sealed class ScrollMenuViewAnimator(float slotHeight, float tweenDuration = 0.2f)
{
    readonly ScrollMenuAnimator _core = new(slotHeight);
    
    public void Apply(
        IReadOnlyList<VisibleSlot> window,
        NavigateDirection direction,
        IReadOnlyList<Control> itemNodes,
        Node tweenHost,
        CancellationToken ct)
    {
        // todo: is currentY necessary?
        var currentY = itemNodes.ToDictionary(
            node => (IScrollMenuItem)node,
            node => node.Position.Y);
        var targets = _core.Compute(window, direction, currentY);
        
        var nodeForItem = itemNodes.ToDictionary(node => (IScrollMenuItem)node);

        foreach (var t in targets.Where(t => t.SnapToY is not null))
        {
            var node = nodeForItem[t.Item];
            node.Position = node.Position with { Y = t.SnapToY!.Value };
        }

        foreach (var t in targets)
        {
            var node = nodeForItem[t.Item];
            node.TweenPositionY(t.TargetY, tweenDuration)
                .PlayAsyncUntilNodeDestroy(tweenHost,ct)
                .Forget();
            node.Modulate = node.Modulate with { A = t.Alpha };
        }
    }
}