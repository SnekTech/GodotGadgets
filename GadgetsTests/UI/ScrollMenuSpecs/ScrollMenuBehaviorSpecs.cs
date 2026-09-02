using AwesomeAssertions;
using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuBehaviorSpecs
{
    [Test]
    public void has_a_focused_item_after_init()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var scrollMenu = new ScrollMenu(new ScrollMenuConfig(items));

        var currentFocused = scrollMenu.CurrentFocused;

        currentFocused.Should().NotBeNull();
        currentFocused.Should().BeAssignableTo<IScrollMenuItem>();
    }

    [Test]
    public void focus_on_the_first_item_by_default()
    {
        var items = MockScrollMenuItem.CreateArray(5);

        var scrollMenu = new ScrollMenu(new ScrollMenuConfig(items));

        scrollMenu.CurrentFocused.Should().BeSameAs(items[0]);
    }

    [Test]
    public void navigate_down_from_1st_item_goes_to_the_2nd_item()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var scrollMenu = new ScrollMenu(new ScrollMenuConfig(items));
        var secondItem = items[1];

        scrollMenu.NavigateDown();

        scrollMenu.CurrentFocused.Should().BeSameAs(secondItem);
    }

    [Test]
    public void visible_window_centers_first_item_with_wrapped_adjacent()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        // focus to the center
        midSlot.Item.Should().BeSameAs(items[0]);
        midSlot.Prominence.Should().Be(ItemProminence.Focused);
        // previous slot wrap to the last item
        prevSlot.Item.Should().BeSameAs(items[^1]);
        prevSlot.Prominence.Should().Be(ItemProminence.Adjacent);
        // next slot is the second item
        nextSlot.Item.Should().BeSameAs(items[1]);
        nextSlot.Prominence.Should().Be(ItemProminence.Adjacent);
    }

    [Test]
    public void navigate_down_shifts_focus_and_visible_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        menu.NavigateDown();
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        midSlot.Item.Should().BeSameAs(items[1]);
        midSlot.Prominence.Should().Be(ItemProminence.Focused);
        prevSlot.Item.Should().BeSameAs(items[0]);
        nextSlot.Item.Should().BeSameAs(items[2]);
    }

    [Test]
    public void navigate_up_shifts_focus_and_visible_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        menu.NavigateDown(); // index -> 1
        menu.NavigateDown(); // index -> 2
        menu.NavigateUp(); // index -> 1
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        midSlot.Item.Should().BeSameAs(items[1]);
        midSlot.Prominence.Should().Be(ItemProminence.Focused);
        prevSlot.Item.Should().BeSameAs(items[0]);
        nextSlot.Item.Should().BeSameAs(items[2]);
    }

    [Test]
    public void navigate_up_from_first_wraps_to_last_with_correct_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        menu.NavigateUp();
        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        midSlot.Item.Should().BeSameAs(items[^1]);
        midSlot.Prominence.Should().Be(ItemProminence.Focused);
        prevSlot.Item.Should().BeSameAs(items[^2]);
        nextSlot.Item.Should().BeSameAs(items[0]);
    }

    [Test]
    public void navigate_down_from_last_wraps_to_first_with_correct_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        // first, jump to last
        menu.NavigateUp();
        menu.NavigateDown();

        var window = menu.VisibleWindow;
        var (prevSlot, midSlot, nextSlot) = (window[0], window[1], window[2]);

        midSlot.Item.Should().BeSameAs(items[0]);
        midSlot.Prominence.Should().Be(ItemProminence.Focused);
        prevSlot.Item.Should().BeSameAs(items[^1]);
        nextSlot.Item.Should().BeSameAs(items[1]);
    }

    [Test]
    public void navigation_returns_focus_moved_effect_with_updated_window()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.NavigateDown();

        var moved = effect as FocusMoved;
        moved.Should().BeOfType<FocusMoved>();
        moved.Window[1].Item.Should().BeSameAs(items[1]);
        moved.Window[1].Prominence.Should().Be(ItemProminence.Focused);
    }

    [Test]
    public void godot_layer_can_react_to_effect_to_update_ui()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new NavigateInput(NavigateDirection.Down));
        var moved = effect as FocusMoved;

        moved.Should().BeOfType<FocusMoved>();
        moved.Window[1].Item.Should().BeSameAs(items[1]);
        moved.Direction.Should().Be(NavigateDirection.Down);
    }
}
