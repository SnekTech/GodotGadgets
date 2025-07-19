namespace GodotGadgets.Extensions;

public static class NodeExtensions
{
    public static void ClearChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
        {
            child.QueueFree();
        }
    }

    public static IEnumerable<T> GetChildrenOfType<T>(this Node node)
    {
        return node.GetChildren().OfType<T>();
    }

    public static void SetStyleBox(this Panel panel, StyleBox styleBox)
    {
        const string panelStylePath = "theme_override_styles/panel";
        panel.Set(panelStylePath, styleBox);
    }

    public static void SetModulateAlpha(this CanvasItem canvasItem, float alpha)
    {
        canvasItem.Modulate = canvasItem.Modulate with { A = alpha };
    }
}