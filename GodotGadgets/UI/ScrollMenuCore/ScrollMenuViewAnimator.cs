using System.Runtime.CompilerServices;
using GodotGadgets.Extensions;
using GodotGadgets.TweenStuff;
using GodotTask;
using GTweensGodot.Extensions;

namespace GodotGadgets.UI.ScrollMenuCore;

/// <summary>
/// Executes ItemAction commands on Godot Control nodes via GTweens.
/// All decision-making is delegated to ScrollMenuAnimator; this class only executes.
/// </summary>
public sealed class ScrollMenuViewAnimator(float slotHeight, float tweenDuration = 0.2f)
{
    readonly ScrollMenuAnimator _core = new(slotHeight);

    public void Apply(
        IReadOnlyList<VisibleSlot> window,
        NavigateDirection direction,
        IReadOnlyList<Control> itemNodes,
        CancellationToken ct)
    {
        var nodeForItem = itemNodes.ToDictionary(node => (IScrollMenuItem)node);
        var allItems = nodeForItem.Keys.ToList();
        var actions = _core.Compute(window, direction, allItems);

        foreach (var itemAction in actions)
        {
            var node = nodeForItem[itemAction.Item];

            _ = itemAction switch
            {
                SlideToSlot slideToSlot => ApplySlide(node, slideToSlot),
                EnterFromEdge enterFromEdge => ApplyEnter(node, enterFromEdge),
                ExitToEdge exitToEdge => ApplyExit(node, exitToEdge),
                StayHidden => ApplyHidden(node),
                _ => throw new SwitchExpressionException(),
            };
        }

        return;

        int ApplySlide(Control node, SlideToSlot slide)
        {
            TweenY(node, slide.TargetY, ct);
            node.SetModulateAlpha(slide.Alpha);
            return 0;
        }

        int ApplyEnter(Control node, EnterFromEdge enterFromEdge)
        {
            node.Position = node.Position with { Y = enterFromEdge.SnapFromY };
            TweenY(node, enterFromEdge.TargetY, ct);
            node.SetModulateAlpha(enterFromEdge.Alpha);
            return 0;
        }

        int ApplyExit(Control node, ExitToEdge exitToEdge)
        {
            TweenY(node, exitToEdge.TargetY, ct);
            node.SetModulateAlpha(0);
            return 0;
        }

        int ApplyHidden(Control node)
        {
            node.SetModulateAlpha(0);
            return 0;
        }
    }

    void TweenY(Control node, float targetY, CancellationToken ct)
    {
        node.TweenPositionY(targetY, tweenDuration)
            .PlayAsyncUntilNodeDestroy(node, ct)
            .Forget();
    }
}