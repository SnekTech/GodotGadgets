namespace GadgetsTests;

public class ScrollMenuSpecs
{
    [Test]
    public async Task Init()
    {
        var x = 1;
        
        await Assert.That(x).IsEqualTo(1);
    }
}