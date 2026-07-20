namespace GodotGadgets.UI.ScrollMenuCore;

public sealed record MenuItemCollection
{
    public MenuItemCollection(IReadOnlyList<IScrollMenuItem> items)
    {
        if (items.Count < 1) throw new ArgumentException("must have at least 1 item to build a scroll menu");
        Items = items;
    }
    
    public IReadOnlyList<IScrollMenuItem> Items { get; }
}

public class ScrollMenu(MenuItemCollection menuItemCollection)
{
    public IScrollMenuItem CurrentFocused { get; private set; }
}

public interface IScrollMenuItem
{
    
}