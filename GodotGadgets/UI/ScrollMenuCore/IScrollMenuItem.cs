namespace GodotGadgets.UI.ScrollMenuCore;

public interface IScrollMenuItem;

public readonly record struct MenuItemBinding(IScrollMenuItem Item, Action OnConfirm);