namespace GodotGadgets.UI.ScrollMenuCore;

public sealed class ScrollMenuAnimator(float slotHeight)
{
    IReadOnlyList<VisibleSlot>? _previousWindow;

    public IReadOnlyList<ItemAction> Compute(
        IReadOnlyList<VisibleSlot> window,
        NavigateDirection direction,
        IReadOnlyList<IScrollMenuItem> allItems)
    {
        var slotted = window.Index().ToDictionary(
            x => x.Item.Item,
            x => new SlottedItem(x.Index, x.Item.Prominence));
        var previousSet = _previousWindow?.Select(s => s.Item).ToHashSet() ?? [];

        var actions = allItems
            .Select(item => Classify(item, slotted, previousSet, window.Count, direction))
            .ToList();

        _previousWindow = window;
        return actions;
    }

    ItemAction Classify(
        IScrollMenuItem item,
        Dictionary<IScrollMenuItem, SlottedItem> slotted,
        HashSet<IScrollMenuItem> previousSet,
        int visibleCount,
        NavigateDirection direction)
    {
        // items in visible window
        if (slotted.TryGetValue(item, out var slot))
        {
            var targetY = slot.Index * slotHeight;
            var alpha = slot.Prominence == ItemProminence.Focused ? 1f : 0.5f;

            return ShouldSnap()
                ? new EnterFromEdge(item, targetY, alpha, EntryEdge())
                : new SlideToSlot(item, targetY, alpha);
        }

        // items just left visible window
        if (previousSet.Contains(item))
        {
            return new ExitToEdge(item, ExitEdge());
        }

        // items not in visible window
        return new StayHidden(item);

        bool ShouldSnap() => previousSet.Count > 0 && !previousSet.Contains(item);
        float EntryEdge() => direction == NavigateDirection.Down ? visibleCount * slotHeight : -slotHeight;
        float ExitEdge() => direction == NavigateDirection.Down ? -slotHeight : visibleCount * slotHeight;
    }
}

readonly record struct SlottedItem(int Index, ItemProminence Prominence);

public abstract record ItemAction(IScrollMenuItem Item);

/// <summary>Item was already visible — just slide to a new slot.</summary>
public sealed record SlideToSlot(IScrollMenuItem Item, float TargetY, float Alpha) : ItemAction(Item);

/// <summary>Item just entered the window — teleport to edge, then slide in.</summary>
public sealed record EnterFromEdge(IScrollMenuItem Item, float TargetY, float Alpha, float SnapFromY) : ItemAction(Item);

/// <summary>Item just left the window — slide off-screen.</summary>
public sealed record ExitToEdge(IScrollMenuItem Item, float TargetY) : ItemAction(Item);

/// <summary>Item is outside the window — no animation.</summary>
public sealed record StayHidden(IScrollMenuItem Item) : ItemAction(Item);
