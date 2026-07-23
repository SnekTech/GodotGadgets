using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuBehaviorSpecs
{
    [Test]
    public async Task has_a_focused_item_after_init()
    {
        var exampleMenuItems = new ScrollMenuConfig([new MockScrollMenuItem()]);

        var scrollMenu = new ScrollMenu(exampleMenuItems);
        var currentFocused = scrollMenu.CurrentFocused;

        await Assert.That(currentFocused)
            .IsNotNull()
            .And
            .IsTypeOf<IScrollMenuItem>();
    }

    [Test]
    public async Task focus_on_the_first_item_by_default()
    {
        var firstItem = new MockScrollMenuItem();
        var secondItem = new MockScrollMenuItem();
        var collection = new ScrollMenuConfig([firstItem, secondItem]);

        var scrollMenu = new ScrollMenu(collection);

        await Assert.That(scrollMenu.CurrentFocused).IsSameReferenceAs(firstItem);
    }

    [Test]
    public async Task navigate_down_from_1st_item_goes_to_the_2nd_item()
    {
        var firstItem = new MockScrollMenuItem();
        var secondItem = new MockScrollMenuItem();
        var collection = new ScrollMenuConfig([firstItem, secondItem]);
        var scrollMenu = new ScrollMenu(collection);

        scrollMenu.NavigateDown();

        await Assert.That(scrollMenu.CurrentFocused).IsSameReferenceAs(secondItem);
    }

    [Test]
    public async Task visible_window_centers_first_item_with_wrapped_adjacent()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items), 3);

        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        // focus to the center
        await Assert.That(midSlot.Item).IsSameReferenceAs(items[0]);
        await Assert.That(midSlot.Prominence).IsEqualTo(ItemProminence.Focused);
        // previous slot wrap to the last item
        await Assert.That(prevSlot.Item).IsSameReferenceAs(items[^1]);
        await Assert.That(prevSlot.Prominence).IsEqualTo(ItemProminence.Adjacent);
        // next slot is the second item
        await Assert.That(nextSlot.Item).IsSameReferenceAs(items[1]);
        await Assert.That(nextSlot.Prominence).IsEqualTo(ItemProminence.Adjacent);
    }

    [Test]
    public async Task navigate_down_shifts_focus_and_visible_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items), 3);

        menu.NavigateDown();
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        await Assert.That(midSlot.Item).IsSameReferenceAs(items[1]);
        await Assert.That(midSlot.Prominence).IsEqualTo(ItemProminence.Focused);
        await Assert.That(prevSlot.Item).IsSameReferenceAs(items[0]);
        await Assert.That(nextSlot.Item).IsSameReferenceAs(items[2]);
    }

    [Test]
    public async Task focus_changed_event_fires_after_navigation()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items), 3);
        var fired = false;
        menu.FocusChanged += () => fired = true;
        
        menu.NavigateDown();

        await Assert.That(fired).IsTrue();
    }
    
    [Test]
    public async Task godot_layer_can_react_to_focus_changed_to_update_ui()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items), 3);
        IReadOnlyList<VisibleSlot>? latestWindow = null;
        menu.FocusChanged += () => latestWindow = menu.VisibleWindow;
        
        menu.NavigateDown();

        await Assert.That(latestWindow).IsNotNull();
        await Assert.That(latestWindow![1].Prominence).IsEqualTo(ItemProminence.Focused);
    }
}