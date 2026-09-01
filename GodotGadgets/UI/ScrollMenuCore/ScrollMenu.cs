using System.Runtime.CompilerServices;

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

    public ScrollMenuEffect Handle(ScrollMenuInput input)
    {
        return input switch
        {
            NavigateInput { Direction: NavigateDirection.Up } => Move(-1, NavigateDirection.Up),
            NavigateInput { Direction: NavigateDirection.Down } => Move(+1, NavigateDirection.Down),
            ConfirmInput => new ConfirmRequested(CurrentFocused),
            ClickInput { Item: var item } when item == CurrentFocused => new ConfirmRequested(item),
            ClickInput { Item: var item } => Focus(item),
            _ => throw new SwitchExpressionException(),
        };
    }

    public ScrollMenuEffect NavigateUp() => Handle(new NavigateInput(NavigateDirection.Up));
    public ScrollMenuEffect NavigateDown() => Handle(new NavigateInput(NavigateDirection.Down));

    ScrollMenuEffect Move(int step, NavigateDirection direction)
    {
        _currentIndex = (_currentIndex + step).Mod(_items.Count);
        return Emit(direction);
    }

    ScrollMenuEffect Focus(IScrollMenuItem item)
    {
        var targetIndex = IndexOf(item);
        var direction = targetIndex < _currentIndex ? NavigateDirection.Up : NavigateDirection.Down;
        _currentIndex = targetIndex;
        return Emit(direction);
    }

    ScrollMenuEffect Emit(NavigateDirection direction)
    {
        var snapshot = BuildVisibleWindow();
        VisibleWindow = snapshot;
        return new FocusMoved(snapshot, direction);
    }

    int IndexOf(IScrollMenuItem item)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            if (ReferenceEquals(_items[i], item))
            {
                return i;
            }
        }

        throw new ArgumentException("clicked item is not a member of this menu");
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
