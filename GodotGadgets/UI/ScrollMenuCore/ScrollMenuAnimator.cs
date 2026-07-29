namespace GodotGadgets.UI.ScrollMenuCore;

public sealed class ScrollMenuAnimator(float slotHeight)
{
    IReadOnlyList<VisibleSlot>? _previousWindow;

    public IReadOnlyList<ItemTarget> Compute(
        IReadOnlyList<VisibleSlot> window,
        NavigateDirection direction,
        IReadOnlyDictionary<IScrollMenuItem, float> currentY)
    {
        var slotted = window.Index().ToDictionary(
            x => x.Item.Item,
            x => new SlottedItem(x.Index, x.Item.Prominence));
        var previousSet = _previousWindow?.Select(s => s.Item).ToHashSet() ?? [];
        var targets = currentY
            .Select(kv => Classify(kv.Key, kv.Value, slotted, previousSet, window.Count, direction))
            .ToList();

        _previousWindow = window;
        return targets;
    }

    ItemTarget Classify(
        IScrollMenuItem item,
        float currentY,
        Dictionary<IScrollMenuItem, SlottedItem> slotted,
        HashSet<IScrollMenuItem> previousSet,
        int visibleCount,
        NavigateDirection direction)
    {
        if (slotted.TryGetValue(item, out var slot))
        {
            var targetY = slot.Index * slotHeight;
            var alpha = slot.Prominence == ItemProminence.Focused ? 1f : 0.5f;
            var snap = ShouldSnap() ? EntryEdge() : (float?)null;
            return new ItemTarget(item, targetY, alpha, snap);
        }

        if (previousSet.Contains(item))
        {
            return new ItemTarget(item, ExitEdge(), 0f, null);
        }

        return new ItemTarget(item, currentY, 0f, null);

        bool ShouldSnap() => previousSet.Count > 0 && !previousSet.Contains(item);
        float EntryEdge() => direction == NavigateDirection.Down ? visibleCount * slotHeight : -slotHeight;
        float ExitEdge() => direction == NavigateDirection.Down ? -slotHeight : visibleCount * slotHeight;
    }
}

public readonly record struct ItemTarget(IScrollMenuItem Item, float TargetY, float Alpha, float? SnapToY);

readonly record struct SlottedItem(int Index, ItemProminence Prominence);