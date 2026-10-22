using AwesomeAssertions;
using Godot;
using GodotGadgets.TooltipSystem;

namespace GadgetsTests.TooltipSpecs;

public class TooltipSideSpecs
{
    static readonly Vector2 Viewport = new(200, 120);
    const float Margin = 8f;

    static TooltipSide Decide(float targetX, float targetY, Vector2 tooltipSize) =>
        TooltipSide.From(new Rect2(targetX, targetY, 40, 16), tooltipSize, Viewport, Margin);

    static Vector2 Tip(float width, float height) => new(width, height);

    [Test]
    public void prefers_the_right_side_and_a_downward_extent_when_both_fit()
    {
        var side = Decide(targetX: 20, targetY: 30, tooltipSize: Tip(80, 40));

        side.Should().Be(TooltipSide.RightDown.Instance);
    }

    [Test]
    public void flips_to_the_left_while_still_extending_downward_when_the_right_side_overflows()
    {
        var side = Decide(targetX: 160, targetY: 30, tooltipSize: Tip(80, 40));

        side.Should().Be(TooltipSide.LeftDown.Instance);
    }

    [Test]
    public void flips_upward_while_staying_on_the_right_when_the_bottom_overflows()
    {
        var side = Decide(targetX: 20, targetY: 100, tooltipSize: Tip(80, 40));

        side.Should().Be(TooltipSide.RightUp.Instance);
    }

    [Test]
    public void flips_to_the_left_and_upward_when_both_axes_overflow()
    {
        var side = Decide(targetX: 160, targetY: 100, tooltipSize: Tip(80, 40));

        side.Should().Be(TooltipSide.LeftUp.Instance);
    }

    [Test]
    public void picks_the_left_side_when_the_tooltip_cannot_fit_on_either_side()
    {
        var side = Decide(targetX: 20, targetY: 30, tooltipSize: Tip(240, 40));

        side.Should().Be(TooltipSide.LeftDown.Instance);
    }

    [Test]
    public void stays_on_the_right_when_the_tooltip_ends_exactly_at_the_viewport_edge()
    {
        var side = Decide(targetX: 20, targetY: 30, tooltipSize: Tip(132, 40));

        side.Should().Be(TooltipSide.RightDown.Instance);
    }

    [Test]
    public void stays_downward_when_the_tooltip_ends_exactly_at_the_viewport_bottom()
    {
        var side = Decide(targetX: 20, targetY: 70, tooltipSize: Tip(80, 50));

        side.Should().Be(TooltipSide.RightDown.Instance);
    }

    [Test]
    public void flips_left_when_the_right_side_overflows_by_half_a_pixel()
    {
        var side = Decide(targetX: 20, targetY: 30, tooltipSize: Tip(132.5f, 40));

        side.Should().Be(TooltipSide.LeftDown.Instance);
    }

    [Test]
    public void flips_upward_when_the_bottom_overflows_by_half_a_pixel()
    {
        var side = Decide(targetX: 20, targetY: 70, tooltipSize: Tip(80, 50.5f));

        side.Should().Be(TooltipSide.RightUp.Instance);
    }
}
