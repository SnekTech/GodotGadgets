using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuSpecs
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

    [Test]
    public async Task throws_when_creating_MenuItemCollection_with_empty_arr()
    {
        IScrollMenuItem[] emptyItemArray = [];
        await Assert.That(() => new MenuItemCollection(emptyItemArray)).Throws<ArgumentException>();
    }

    [Test]
    public async Task creates_successfully_with_valid_items()
    {
        IScrollMenuItem[] threeItems = [new MockScrollMenuItem(), new MockScrollMenuItem(), new MockScrollMenuItem()];
        
        var menuItemCollection = new MenuItemCollection(threeItems);
        
        await Assert.That(menuItemCollection.Items.Count).IsEqualTo(threeItems.Length);
    }
    
    // [Test]
    // public async Task navigate_up_from_1st_item_goes_to_the_last()
    // {
    //     await Assert.That()
    // }
}