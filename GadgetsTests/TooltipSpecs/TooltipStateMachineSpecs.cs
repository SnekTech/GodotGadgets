using AwesomeAssertions;
using Godot;
using GodotGadgets.TooltipSystem;
using Entered = GodotGadgets.TooltipSystem.TooltipState.Input.Entered;
using Exited = GodotGadgets.TooltipSystem.TooltipState.Input.Exited;
using Tick = GodotGadgets.TooltipSystem.TooltipState.Input.Tick;

namespace GadgetsTests.TooltipSpecs;

public class TooltipStateMachineSpecs
{
    static readonly TooltipConfig DefaultConfig = TooltipConfig.Default;

    static readonly HoverKey Key1 = new(1);
    static readonly HoverKey Key2 = new(2);

    static TimeSpan TimeAt(int milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

    static TooltipView View(HoverKey key) => new(key,
        new TooltipContent("fake title", "fake content"), new Rect2(0, 0, 10, 10));

    static HoverRequest Req(HoverKey key, TimeSpan timeAt) => new(View(key), timeAt);

    [Test]
    public void hover_waits_for_the_delay_instead_of_showing_immediately()
    {
        var hidden = TooltipState.Hidden.Instance;
        var (next, presentation) = hidden.Update(TooltipConfig.Default, TimeAt(1), new Entered(View(Key1)));

        next.Should().BeOfType<TooltipState.Waiting>();
        presentation.Should().BeNull();
    }

    [Test]
    public void leaving_before_the_delay_elapses_cancels_and_shows_nothing()
    {
        var waiting = new TooltipState.Waiting(Req(Key1, TimeAt(1)));

        var (next, presentation) = waiting.Update(DefaultConfig, TimeAt(2), new Exited(Key1));

        next.Should().Be(TooltipState.Hidden.Instance);
        presentation.Should().BeNull();
    }

    [Test]
    public void stale_exit_from_an_earlier_hover_is_ignored()
    {
        var shown = new TooltipState.Shown(Req(Key2, TimeAt(1)));

        var (next, presentation) = shown.Update(DefaultConfig, TimeAt(2), new Exited(Key1));

        next.Should().BeOfType<TooltipState.Shown>()
            .Which.Request.View.Key.Should().Be(Key2);
        presentation.Should().Be(View(Key2));
    }

    [Test]
    public void moving_to_another_target_while_visible_switches_without_waiting()
    {
        var shown = new TooltipState.Shown(Req(Key1, TimeAt(1)));

        var (next, presentation) = shown.Update(DefaultConfig, TimeAt(2), new Entered(View(Key2)));

        next.Should().BeOfType<TooltipState.Shown>()
            .Which.Request.View.Key.Should().Be(Key2);
        presentation.Should().Be(View(Key2));
    }

    [Test]
    public void zero_delay_shows_immediately()
    {
        var hidden = TooltipState.Hidden.Instance;
        var zeroDelayConfig = DefaultConfig with { HoverDelay = TimeSpan.Zero };

        var (next, presentation) = hidden.Update(zeroDelayConfig, TimeAt(1), new Entered(View(Key1)));

        next.Should().BeOfType<TooltipState.Shown>();
        presentation.Should().Be(View(Key1));
    }

    [Test]
    public void exit_when_visible()
    {
        var shown = new TooltipState.Shown(Req(Key1, TimeAt(1)));
        var (next, presentation) = shown.Update(DefaultConfig, TimeAt(2), new Exited(Key1));

        next.Should().Be(TooltipState.Hidden.Instance);
        presentation.Should().BeNull();
    }

    [Test]
    public void enter_when_waiting_for_hover_should_start_a_new_waiting()
    {
        var waiting = new TooltipState.Waiting(Req(Key1, TimeAt(1)));

        var (next, presentation) = waiting.Update(DefaultConfig, TimeAt(2), new Entered(View(Key2)));

        next.Should().BeOfType<TooltipState.Waiting>()
            .Which.Request.View.Key.Should().Be(Key2);
        presentation.Should().BeNull();
    }

    [Test]
    public void early_tick_on_waiting_should_show_nothing()
    {
        var waiting = new TooltipState.Waiting(Req(Key1, TimeAt(1)));

        var (next, presentation) = waiting.Update(DefaultConfig, TimeAt(2), Tick.Instance);

        next.Should().BeOfType<TooltipState.Waiting>();
        presentation.Should().BeNull();
    }


    [Test]
    public void tick_on_waiting_on_time_should_show()
    {
        var waiting = new TooltipState.Waiting(Req(Key1, TimeAt(1)));
        var timeLongEnough = TimeAt(1) + DefaultConfig.HoverDelay;

        var (next, presentation) = waiting.Update(DefaultConfig, timeLongEnough, Tick.Instance);

        next.Should().BeOfType<TooltipState.Shown>();
        presentation.Should().Be(View(Key1));
    }

    [Test]
    public void tick_on_shown_should_output_the_same_shown()
    {
        var shown = new TooltipState.Shown(Req(Key1, TimeAt(1)));

        var (next, presentation) = shown.Update(DefaultConfig, TimeAt(2), Tick.Instance);

        next.Should().Be(shown);
        presentation.Should().Be(View(Key1));
    }

    [Test]
    public void exit_twice_should_not_change_output()
    {
        var shown = new TooltipState.Shown(Req(Key1, TimeAt(1)));

        var (hidden, _) = shown.Update(DefaultConfig, TimeAt(2), new Exited(Key1));
        var (again, presentation) = hidden.Update(DefaultConfig, TimeAt(3), new Exited(Key1));

        again.Should().Be(TooltipState.Hidden.Instance);
        presentation.Should().BeNull();
    }

    [Test]
    public void from_null_to_view_should_show()
    {
        var view = View(Key1);

        var transition = TooltipTransition.From(null, view);

        transition.Should().Be(new TooltipTransition.Show(view));
    }

    [Test]
    public void from_view_to_same_view_should_remain_unchanged()
    {
        var view1 = View(Key1);

        var transition = TooltipTransition.From(view1, view1);
        var nullToNullTransition = TooltipTransition.From(null, null);

        transition.Should().Be(new TooltipTransition.Unchanged());
        nullToNullTransition.Should().Be(new TooltipTransition.Unchanged());
    }
    
    [Test]
    public void from_view_to_view_should_replace_with_new_view()
    {
        var view1 = View(Key1);
        var view2 = View(Key2);

        var transition = TooltipTransition.From(view1, view2);

        transition.Should().Be(new TooltipTransition.Replace(view2));
    }

    [Test]
    public void from_view_to_null_should_hide()
    {
        var view = View(Key1);

        var transition = TooltipTransition.From(view, null);

        transition.Should().Be(new TooltipTransition.Hide());
    }
}