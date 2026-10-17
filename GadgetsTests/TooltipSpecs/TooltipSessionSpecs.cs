using AwesomeAssertions;
using Godot;
using GodotGadgets.TooltipSystem;
using static GodotGadgets.TooltipSystem.TooltipSession;

namespace GadgetsTests.TooltipSpecs;

public class TooltipSessionSpecs
{
    static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(350);

    static TooltipRequest Req(long id) =>
        new(new RequestId(id), TooltipContent.New("title", "content"), new Rect2(0, 0, 10, 10));

    [Test]
    public void hover_waits_for_the_delay_instead_of_showing_immediately()
    {
        var hidden = new Hidden();
        var (session, effects) = hidden.Update(new Event.PointerEntered(Req(1)), Delay);

        session.Should().BeOfType<WaitingForHover>();
        effects.Should().ContainSingle().Which.Should().BeOfType<Effect.StartHoverDelay>();
    }

    [Test]
    public void leaving_before_the_delay_elapses_cancels_and_shows_nothing()
    {
        var hidden = new Hidden();
        var (waiting, _) = hidden.Update(new Event.PointerEntered(Req(1)), Delay);
        var (session, effects) = waiting.Update(new Event.PointerExited(), Delay);

        session.Should().BeOfType<Hidden>();
        effects.Should().ContainSingle().Which.Should().BeOfType<Effect.CancelHoverDelay>();
        effects.Should().NotContain(x => x is Effect.Show);
    }

    [Test]
    public void stale_timer_from_an_earlier_hover_is_ignored()
    {
        var hidden = new Hidden();
        var (waiting, _) = hidden.Update(new Event.PointerEntered(Req(2)), Delay);
        var (session, effects) = waiting.Update(new Event.HoverDelayElapsed(new RequestId(1)), Delay);

        session.Should().BeOfType<WaitingForHover>();
        effects.Should().BeEmpty();
    }

    [Test]
    public void moving_to_another_target_while_visible_switches_without_waiting()
    {
        var visible = new Visible(Req(1));

        var (session, effects) = visible.Update(new Event.PointerEntered(Req(2)), Delay);

        session.Should().Be(new Visible(Req(2)));
        effects.Should().ContainSingle().Which.Should().BeOfType<Effect.Show>();
    }

    [Test]
    public void zero_delay_shows_immediately()
    {
        var hidden = new Hidden();
        var (session, effects) = hidden.Update(new Event.PointerEntered(Req(1)), TimeSpan.Zero);

        session.Should().BeOfType<Visible>();
        effects.Should().ContainSingle().Which.Should().BeOfType<Effect.Show>();
    }

    [Test]
    public void exit_when_visible_should_fade_out()
    {
        var visible = new Visible(Req(1));
        var (session, effects) = visible.Update(new Event.PointerExited(), Delay);

        session.Should().BeOfType<Hidden>();
        effects.Should().ContainSingle().Which.Should().BeOfType<Effect.FadeOut>();
    }

    [Test]
    public void enter_when_waiting_for_hover_should_start_a_new_waiting()
    {
        var waiting = new WaitingForHover(Req(1));
        var (session, effects) = waiting.Update(new Event.PointerEntered(Req(2)), Delay);

        session.Should().BeOfType<WaitingForHover>()
            .Which.Request.Should().Be(Req(2));
        effects.Should().ContainInOrder([new Effect.CancelHoverDelay(), new Effect.StartHoverDelay(Delay, Req(2).Id)]);
    }
}