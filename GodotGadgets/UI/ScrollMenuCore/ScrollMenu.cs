namespace GodotGadgets.UI.ScrollMenuCore;

public sealed class ScrollMenu
{
    readonly ScrollMenuConfig _items;
    readonly int _visibleCount;
    int _currentIndex;
    VisibleSlot[] _visibleWindow;
    
    public ScrollMenu(ScrollMenuConfig scrollMenuConfig, int visibleCount = 3)
    {
        _items = scrollMenuConfig;
    }

    public event Action? FocusChanged;
    
    public IScrollMenuItem CurrentFocused { get; private set; }
    public IReadOnlyList<VisibleSlot> VisibleWindow { get; private set; } = [];

    public void NavigateDown()
    {
        throw new NotImplementedException();
    }

    void BuildVisibleWindow()
    {
        
    }
}

public readonly record struct VisibleSlot(IScrollMenuItem Item, ItemProminence Prominence);

public enum ItemProminence
{
    Focused,
    Adjacent,
    Hidden,
}