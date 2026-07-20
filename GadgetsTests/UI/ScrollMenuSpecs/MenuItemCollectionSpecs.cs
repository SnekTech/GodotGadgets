using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class MenuItemCollectionSpecs
{
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
}