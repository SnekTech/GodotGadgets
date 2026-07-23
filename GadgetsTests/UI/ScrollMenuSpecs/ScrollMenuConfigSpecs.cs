using GodotGadgets.UI.ScrollMenuCore;
using TUnit.Assertions.Should;
using TUnit.Assertions.Should.Extensions;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuConfigSpecs
{
    [Test]
    public async Task throws_when_creating_MenuItemCollection_with_empty_arr()
    {
        IScrollMenuItem[] emptyItemArray = [];
        await Assert.That(() => new ScrollMenuConfig(emptyItemArray)).Throws<ArgumentException>();
    }

    [Test]
    public async Task creates_successfully_with_valid_items()
    {
        IScrollMenuItem[] threeItems = [new MockScrollMenuItem(), new MockScrollMenuItem(), new MockScrollMenuItem()];

        var menuItemCollection = new ScrollMenuConfig(threeItems);

        await Assert.That(menuItemCollection.Items.Count).IsEqualTo(threeItems.Length);
    }

    [Test]
    public async Task accepts_valid_odd_visible_count_within_item_count()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        const int visibleCount = 5;
        var config = new ScrollMenuConfig(items, visibleCount);

        await config.VisibleCount.Should().BeEqualTo(visibleCount);
    }

    [Test]
    public async Task visible_count_can_equal_item_count()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        const int visibleCount = 5;
        var config = new ScrollMenuConfig(items, visibleCount);

        await config.VisibleCount.Should().BeEqualTo(visibleCount);
    }

    [Test]
    public async Task visible_count_of_1_is_always_valid_when_item_exist()
    {
        var items = MockScrollMenuItem.CreateArray(2);
        const int visibleCount = 1;
        var config = new ScrollMenuConfig(items, visibleCount);

        await config.VisibleCount.Should().BeEqualTo(visibleCount);
    }

    [Test]
    public async Task throws_when_visible_count_exceeds_items_count()
    {
        var items = MockScrollMenuItem.CreateArray(3);
        const int visibleCount = 5;

        await Assert.That(() => new ScrollMenuConfig(items, visibleCount)).Throws<ArgumentException>();
    }
    
    [Test]
    public async Task throws_when_visible_count_is_even()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        const int visibleCount = 4;

        await Assert.That(() => new ScrollMenuConfig(items, visibleCount)).Throws<ArgumentException>();
    }
}