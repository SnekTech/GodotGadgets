using GodotGadgets.UI.ScrollMenuCore;
using TUnit.Assertions.Should;
using TUnit.Assertions.Should.Extensions;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuInputSpecs
{
    [Test]
    public async Task navigate_down_input_moves_focus_and_returns_focus_moved()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new NavigateInput(NavigateDirection.Down));

        await menu.CurrentFocused.Should().BeSameReferenceAs(items[1]);
        await Assert.That(effect).IsTypeOf<FocusMoved>();
    }

    [Test]
    public async Task navigate_up_input_wraps_to_last_item()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new NavigateInput(NavigateDirection.Up));

        await menu.CurrentFocused.Should().BeSameReferenceAs(items[^1]);
        await Assert.That(effect).IsTypeOf<FocusMoved>();
    }

    [Test]
    public async Task click_on_focused_item_requests_confirm_without_moving_focus()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new ClickInput(items[0]));

        var confirm = effect as ConfirmRequested;
        await confirm.Should().NotBeNull();
        await confirm!.Item.Should().BeSameReferenceAs(items[0]);
        await menu.CurrentFocused.Should().BeSameReferenceAs(items[0]);
    }

    [Test]
    public async Task click_on_non_focused_item_moves_focus_and_does_not_confirm()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new ClickInput(items[1]));

        await menu.CurrentFocused.Should().BeSameReferenceAs(items[1]);
        await Assert.That(effect).IsTypeOf<FocusMoved>();
    }

    [Test]
    public async Task click_on_item_below_focus_reports_down_direction()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));

        var effect = menu.Handle(new ClickInput(items[2]));
        var moved = effect as FocusMoved;

        await moved.Should().NotBeNull();
        await moved!.Direction.Should().BeEqualTo(NavigateDirection.Down);
    }

    [Test]
    public async Task click_on_item_above_focus_reports_up_direction()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        menu.NavigateDown();
        menu.NavigateDown(); // focus -> items[2]

        var effect = menu.Handle(new ClickInput(items[0]));
        var moved = effect as FocusMoved;

        await moved.Should().NotBeNull();
        await moved!.Direction.Should().BeEqualTo(NavigateDirection.Up);
    }

    [Test]
    public async Task confirm_input_requests_confirmation_for_focused_item()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        menu.NavigateDown();

        var effect = menu.Handle(new ConfirmInput());

        var confirm = effect as ConfirmRequested;
        await confirm.Should().NotBeNull();
        await confirm!.Item.Should().BeSameReferenceAs(items[1]);
    }

    [Test]
    public async Task click_on_item_not_in_menu_throws()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        var menu = new ScrollMenu(new ScrollMenuConfig(items));
        var foreign = new MockScrollMenuItem();

        var act = () => menu.Handle(new ClickInput(foreign));

        await Assert.That(act).Throws<ArgumentException>();
    }
}
