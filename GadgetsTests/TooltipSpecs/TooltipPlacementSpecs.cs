using AwesomeAssertions;
using Godot;
using GodotGadgets.TooltipSystem;

namespace GadgetsTests.TooltipSpecs;

public class TooltipPlacementSpecs
{
    static readonly Vector2 Viewport = new(200, 120);
    static readonly Vector2[] Tooltips = [new(20, 10), new(80, 40), new(132, 90), new(240, 200)];
    const float Margin = 8f;

    /// <summary>把 target 铺满视口，覆盖"贴边 / 恰好 / 差一点"的所有位置。</summary>
    static IEnumerable<Rect2> Targets() =>
        from x in Steps(0, Viewport.X - 30)
        from y in Steps(0, Viewport.Y - 20)
        select new Rect2(x, y, 30, 20);

    static IEnumerable<float> Steps(float from, float to)
    {
        for (var value = from; value <= to; value += 10) yield return value;
    }

    static Vector2 Place(Rect2 target, Vector2 tooltip) =>
        TooltipPlacement.Resolve(target, tooltip, Viewport, Margin);

    [Test]
    public void the_tooltip_never_ends_up_outside_the_viewport()
    {
        foreach (var target in Targets())
        foreach (var tooltip in Tooltips)
        {
            var position = Place(target, tooltip);

            position.X.Should().BeInRange(0f, float.Max(0f, Viewport.X - tooltip.X), Situation(target, tooltip));
            position.Y.Should().BeInRange(0f, float.Max(0f, Viewport.Y - tooltip.Y), Situation(target, tooltip));
        }
    }

    [Test]
    public void a_side_that_has_room_keeps_its_exact_gap_to_the_target()
    {
        foreach (var target in Targets())
        foreach (var tooltip in Tooltips)
        {
            var position = Place(target, tooltip);

            if (target.End.X + Margin + tooltip.X <= Viewport.X)
            {
                position.X.Should().Be(target.End.X + Margin, Situation(target, tooltip));
            }

            if (target.Position.Y + tooltip.Y <= Viewport.Y)
            {
                position.Y.Should().Be(target.Position.Y, Situation(target, tooltip));
            }
        }
    }

    static string Situation(Rect2 target, Vector2 tooltip) => $"target {target} tooltip {tooltip}";
}
