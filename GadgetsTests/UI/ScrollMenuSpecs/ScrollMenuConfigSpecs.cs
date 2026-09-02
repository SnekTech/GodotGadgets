using AwesomeAssertions;
using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

public class ScrollMenuConfigSpecs
{
    [Test]
    public void throws_when_creating_MenuItemCollection_with_empty_arr()
    {
        IScrollMenuItem[] emptyItemArray = [];
        var createFn = () => new ScrollMenuConfig(emptyItemArray);
        createFn.Should().Throw<ArgumentException>();
    }

    [Test]
    public void creates_successfully_with_valid_items()
    {
        IScrollMenuItem[] threeItems = [new MockScrollMenuItem(), new MockScrollMenuItem(), new MockScrollMenuItem()];

        var menuItemCollection = new ScrollMenuConfig(threeItems);

        menuItemCollection.Items.Count.Should().Be(threeItems.Length);
    }

    [Test]
    public void accepts_valid_odd_visible_count_within_item_count()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        const int visibleCount = 5;
        var config = new ScrollMenuConfig(items, visibleCount);

        config.VisibleCount.Should().Be(visibleCount);
    }

    [Test]
    public void visible_count_can_equal_item_count()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        const int visibleCount = 5;
        var config = new ScrollMenuConfig(items, visibleCount);

        config.VisibleCount.Should().Be(visibleCount);
    }

    [Test]
    public void visible_count_of_1_is_always_valid_when_item_exist()
    {
        var items = MockScrollMenuItem.CreateArray(2);
        const int visibleCount = 1;
        var config = new ScrollMenuConfig(items, visibleCount);

        config.VisibleCount.Should().Be(visibleCount);
    }

    [Test]
    public void throws_when_visible_count_exceeds_items_count()
    {
        var items = MockScrollMenuItem.CreateArray(3);
        const int visibleCount = 5;

        var createFn = () => new ScrollMenuConfig(items, visibleCount);
        createFn.Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void throws_when_visible_count_is_even()
    {
        var items = MockScrollMenuItem.CreateArray(5);
        const int visibleCount = 4;

        var createFn = () => new ScrollMenuConfig(items, visibleCount);
        createFn.Should().Throw<ArgumentException>();
    }
}