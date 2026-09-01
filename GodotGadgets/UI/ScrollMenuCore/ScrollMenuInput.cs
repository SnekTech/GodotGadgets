namespace GodotGadgets.UI.ScrollMenuCore;

public abstract record ScrollMenuInput;

/// <summary>Directional step navigation: keyboard arrows, mouse wheel, stick, d-pad.</summary>
public sealed record NavigateInput(NavigateDirection Direction) : ScrollMenuInput;

/// <summary>Pointer click on a specific item. The domain decides focus vs confirm.</summary>
public sealed record ClickInput(IScrollMenuItem Item) : ScrollMenuInput;

/// <summary>Explicit confirm: keyboard Enter / gamepad A.</summary>
public sealed record ConfirmInput : ScrollMenuInput;
