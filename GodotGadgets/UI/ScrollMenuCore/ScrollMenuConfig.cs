namespace GodotGadgets.UI.ScrollMenuCore;

public sealed record ScrollMenuConfig
{
    public IReadOnlyList<IScrollMenuItem> Items { get; }
    public int VisibleCount { get; }

    public ScrollMenuConfig(IReadOnlyList<IScrollMenuItem> items, int visibleCount = 3)
    {
        if (items.Count < 1)
        {
            throw new ArgumentException("must have at least 1 item to build a scroll menu");
        }

        if (visibleCount < 1 || visibleCount % 2 == 0 || visibleCount > items.Count)
        {
            throw new ArgumentException("visibleCount must be a positive odd number not exceeding item count");
        }

        Items = items;
        VisibleCount = visibleCount;
    }
}