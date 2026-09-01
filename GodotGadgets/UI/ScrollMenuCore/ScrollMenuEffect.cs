namespace GodotGadgets.UI.ScrollMenuCore;

public abstract record ScrollMenuEffect;

/// <summary>Focus moved; reflow the window with this snapshot.</summary>
public sealed record FocusMoved(IReadOnlyList<VisibleSlot> Window, NavigateDirection Direction) : ScrollMenuEffect;

/// <summary>Confirm fired; execute the binding for this item.</summary>
public sealed record ConfirmRequested(IScrollMenuItem Item) : ScrollMenuEffect;
