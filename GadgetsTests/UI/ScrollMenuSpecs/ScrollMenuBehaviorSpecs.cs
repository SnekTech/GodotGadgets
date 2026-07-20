using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuBehaviorSpecs
{
    [Test]
    public async Task has_a_focused_item_after_init()
    {
        var exampleMenuItems = new MenuItemCollection([new MockScrollMenuItem()]);

        var scrollMenu = new ScrollMenu(exampleMenuItems);
        var currentFocused = scrollMenu.CurrentFocused;

        await Assert.That(currentFocused)
            .IsNotNull()
            .And
            .IsTypeOf<IScrollMenuItem>();
    }

    
    // [Test]
    // public async Task navigate_up_from_1st_item_goes_to_the_last()
    // {
    //     await Assert.That()
    // }
}