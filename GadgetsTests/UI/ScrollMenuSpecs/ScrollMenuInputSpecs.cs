using AwesomeAssertions;
using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuInputSpecs
{
    [Test]
    public void navigate_down_input_moves_focus_and_returns_focus_moved()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new NavigateInput(NavigateDirection.Down));

        menu.CurrentFocused.Should().BeSameAs(items[1]);
        effect.Should().BeOfType<FocusMoved>();
    }

    [Test]
    public void navigate_up_input_wraps_to_last_item()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new NavigateInput(NavigateDirection.Up));

        menu.CurrentFocused.Should().BeSameAs(items[^1]);
        effect.Should().BeOfType<FocusMoved>();
    }

    [Test]
    public void click_on_focused_item_requests_confirm_without_moving_focus()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new ClickInput(items[0]));

        var confirm = effect as ConfirmRequested;
        confirm.Should().BeOfType<ConfirmRequested>();
        confirm.Item.Should().BeSameAs(items[0]);
        menu.CurrentFocused.Should().BeSameAs(items[0]);
    }

    [Test]
    public void click_on_non_focused_item_moves_focus_and_does_not_confirm()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new ClickInput(items[1]));

        menu.CurrentFocused.Should().BeSameAs(items[1]);
        effect.Should().NotBeOfType<ConfirmRequested>();
    }

    [Test]
    public void click_on_item_below_focus_reports_down_direction()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new ClickInput(items[2]));
        var moved = effect as FocusMoved;

        moved.Should().BeOfType<FocusMoved>();
        moved.Direction.Should().Be(NavigateDirection.Down);
    }

    [Test]
    public void click_on_item_above_focus_reports_up_direction()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        menu.NavigateDown();
        menu.NavigateDown(); // focus -> items[2]

        var effect = menu.Handle(new ClickInput(items[0]));
        var moved = effect as FocusMoved;

        moved.Should().BeOfType<FocusMoved>();
        moved.Direction.Should().Be(NavigateDirection.Up);
    }

    [Test]
    public void confirm_input_requests_confirmation_for_focused_item()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        menu.NavigateDown();

        var effect = menu.Handle(new ConfirmInput());

        var confirm = effect as ConfirmRequested;
        confirm.Should().BeOfType<ConfirmRequested>();
        confirm.Item.Should().BeSameAs(items[1]);
    }

    [Test]
    public void click_on_item_not_in_menu_throws()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        var foreign = new MockScrollMenuItem();

        var handleFn = () => menu.Handle(new ClickInput(foreign));

        handleFn.Should().Throw<ArgumentException>();
    }
}
