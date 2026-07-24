using GodotGadgets.UI.ScrollMenuCore;
using TUnit.Assertions.Should;
using TUnit.Assertions.Should.Extensions;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuBehaviorSpecs
{
    [Test]
    public async Task has_a_focused_item_after_init()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var scrollMenu = new ScrollMenu(new ScrollMenuConfig(items));

        var currentFocused = scrollMenu.CurrentFocused;

        await Assert.That(currentFocused)
            .IsNotNull()
            .And
            .IsTypeOf<IScrollMenuItem>();
    }

    [Test]
    public async Task focus_on_the_first_item_by_default()
    {
        var items = MockScrollMenuItem.CreateArray(5);

        var scrollMenu = new ScrollMenu(new ScrollMenuConfig(items));

        await scrollMenu.CurrentFocused.Should().BeSameReferenceAs(items[0]);
    }

    [Test]
    public async Task navigate_down_from_1st_item_goes_to_the_2nd_item()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var scrollMenu = new ScrollMenu(new ScrollMenuConfig(items));
        var secondItem = items[1];

        scrollMenu.NavigateDown();

        await scrollMenu.CurrentFocused.Should().BeSameReferenceAs(secondItem);
    }

    [Test]
    public async Task visible_window_centers_first_item_with_wrapped_adjacent()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        // focus to the center
        await midSlot.Item.Should().BeSameReferenceAs(items[0]);
        await midSlot.Prominence.Should().BeEqualTo(ItemProminence.Focused);
        // previous slot wrap to the last item
        await prevSlot.Item.Should().BeSameReferenceAs(items[^1]);
        await prevSlot.Prominence.Should().BeEqualTo(ItemProminence.Adjacent);
        // next slot is the second item
        await nextSlot.Item.Should().BeSameReferenceAs(items[1]);
        await nextSlot.Prominence.Should().BeEqualTo(ItemProminence.Adjacent);
    }

    [Test]
    public async Task navigate_down_shifts_focus_and_visible_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        menu.NavigateDown();
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        await midSlot.Item.Should().BeSameReferenceAs(items[1]);
        await midSlot.Prominence.Should().BeEqualTo(ItemProminence.Focused);
        await prevSlot.Item.Should().BeSameReferenceAs(items[0]);
        await nextSlot.Item.Should().BeSameReferenceAs(items[2]);
    }

    [Test]
    public async Task navigate_up_shifts_focus_and_visible_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        menu.NavigateDown(); // index -> 1
        menu.NavigateDown(); // index -> 2
        menu.NavigateUp(); // index -> 1
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        await midSlot.Item.Should().BeSameReferenceAs(items[1]);
        await midSlot.Prominence.Should().BeEqualTo(ItemProminence.Focused);
        await prevSlot.Item.Should().BeSameReferenceAs(items[0]);
        await nextSlot.Item.Should().BeSameReferenceAs(items[2]);
    }

    [Test]
    public async Task navigate_up_from_first_wraps_to_last_with_correct_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        menu.NavigateUp();
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        await midSlot.Item.Should().BeSameReferenceAs(items[^1]);
        await midSlot.Prominence.Should().BeEqualTo(ItemProminence.Focused);
        await prevSlot.Item.Should().BeSameReferenceAs(items[^2]);
        await nextSlot.Item.Should().BeSameReferenceAs(items[0]);
    }

    [Test]
    public async Task navigate_down_from_last_wraps_to_first_with_correct_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        // first, jump to last
        menu.NavigateUp();
        menu.NavigateDown();

        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        await midSlot.Item.Should().BeSameReferenceAs(items[0]);
        await midSlot.Prominence.Should().BeEqualTo(ItemProminence.Focused);
        await prevSlot.Item.Should().BeSameReferenceAs(items[^1]);
        await nextSlot.Item.Should().BeSameReferenceAs(items[1]);
    }


    [Test]
    public async Task focus_changed_event_fires_after_navigation()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        var fired = false;
        menu.FocusChanged += (_, _) => fired = true;

        menu.NavigateDown();

        await fired.Should().BeTrue();
    }

    [Test]
    public async Task godot_layer_can_react_to_focus_changed_to_update_ui()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        IReadOnlyList<VisibleSlot>? latestWindow = null;
        menu.FocusChanged += (window, _) => latestWindow = window;

        menu.NavigateDown();

        await latestWindow.Should().NotBeNull();
        await latestWindow![1].Prominence.Should().BeEqualTo(ItemProminence.Focused);
    }
}