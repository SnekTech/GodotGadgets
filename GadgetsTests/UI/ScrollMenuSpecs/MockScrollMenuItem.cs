using GodotGadgets.UI.ScrollMenuCore;

namespace GadgetsTests.UI.ScrollMenuSpecs;

class MockScrollMenuItem : IScrollMenuItem;

static class MockScrollMenuItemExtensions
{
    extension(MockScrollMenuItem)
    {
        internal static IScrollMenuItem[] CreateArray(int count) =>
            Enumerable.Range(0, count).Select(_ => new MockScrollMenuItem()).ToArray<IScrollMenuItem>();
    }
}