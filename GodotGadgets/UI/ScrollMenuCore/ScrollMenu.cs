namespace GodotGadgets.UI.ScrollMenuCore;

public sealed class ScrollMenu
{
    readonly IReadOnlyList<IScrollMenuItem> _items;
    readonly int _visibleCount;
    int _currentIndex;

    public ScrollMenu(ScrollMenuConfig scrollMenuConfig)
    {
        _items = scrollMenuConfig.Items;
        _visibleCount = scrollMenuConfig.VisibleCount;
        VisibleWindow = BuildVisibleWindow();
    }

    public IScrollMenuItem CurrentFocused => _items[_currentIndex];
    public IReadOnlyList<VisibleSlot> VisibleWindow { get; private set; }
    public event Action<IReadOnlyList<VisibleSlot>, NavigateDirection>? FocusChanged;

    public void NavigateDown()
    {
        _currentIndex = (_currentIndex + 1).Mod(_items.Count);
        EmitChange(NavigateDirection.Down);
    }

    public void NavigateUp()
    {
        _currentIndex = (_currentIndex - 1).Mod(_items.Count);
        EmitChange(NavigateDirection.Up);
    }

    void EmitChange(NavigateDirection direction)
    {
        var snapshot = BuildVisibleWindow();
        VisibleWindow = snapshot;
        FocusChanged?.Invoke(snapshot, direction);
    }

    VisibleSlot[] BuildVisibleWindow()
    {
        var half = _visibleCount / 2;
        var start = _currentIndex - half;
        var window = new VisibleSlot[_visibleCount];

        for (int slotIndex = 0; slotIndex < _visibleCount; slotIndex++)
        {
            var itemIndex = (start + slotIndex).Mod(_items.Count);
            window[slotIndex] = new VisibleSlot(_items[itemIndex],
                slotIndex == half ? ItemProminence.Focused : ItemProminence.Adjacent);
        }

        return window;
    }
}

public enum NavigateDirection
{
    Up,
    Down,
}

public readonly record struct VisibleSlot(IScrollMenuItem Item, ItemProminence Prominence);

public enum ItemProminence
{
    Focused,
    Adjacent,
}

file static class IntExtensions
{
    internal static int Mod(this int x, int m) => (x % m + m) % m;
}