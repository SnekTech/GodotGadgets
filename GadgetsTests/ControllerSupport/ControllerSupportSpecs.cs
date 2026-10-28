using AwesomeAssertions;
using GodotGadgets.InputProcessing;

namespace GadgetsTests.ControllerSupport;

public sealed class ControllerSupportSpecs
{
    static readonly Gamepad Xbox = new(GamepadBrand.Xbox);
    
    [Test]
    public void negative_axis_value_should_change_device_to_gamepad()
    {
        var detector = new DeviceDetector();
        
        detector.Handle(new InputSample.GamepadAxis(Xbox, -0.9f));

        detector.Current.Should().BeOfType<Gamepad>();
    }
    
    [Test]
    public void axis_value_in_deadzone_should_not_change_device_and_not_trigger_event()
    {
        var detector = new DeviceDetector();
        using var monitoredDetector = detector.Monitor();
        
        detector.Handle(new InputSample.GamepadAxis(Xbox, -0.1f));

        detector.Current.Should().NotBeOfType<Gamepad>();
        monitoredDetector.Should().NotRaise(nameof(detector.DeviceChanged));
    }
    
    [Test]
    public void two_samples_from_the_same_device_only_trigger_changed_once()
    {
        var detector = new DeviceDetector();
        using var monitoredDetector = detector.Monitor();
        
        detector.Handle(new InputSample.GamepadButton(Xbox));
        detector.Handle(new InputSample.GamepadButton(Xbox));

        monitoredDetector.OccurredEvents
            .Where(e => e.EventName == nameof(detector.DeviceChanged))
            .Should().HaveCount(1);
    }

    [Test]
    public void device_flow_of_gamepad_mouse_gamepad()
    {
        var detector = new DeviceDetector();
        
        detector.Handle(new InputSample.GamepadButton(Xbox));
        detector.Current.Should().BeOfType<Gamepad>();
        detector.Handle(new InputSample.Mouse());
        detector.Current.Should().BeOfType<Mouse>();
        detector.Handle(new InputSample.GamepadButton(Xbox));
        detector.Current.Should().BeOfType<Gamepad>();
    }
}