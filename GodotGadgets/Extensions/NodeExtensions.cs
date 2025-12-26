namespace GodotGadgets.Extensions;

public static class NodeExtensions
{
    extension(Node node)
    {
        [UsedImplicitly]
        public void ClearChildren()
        {
            foreach (var child in node.GetChildren())
            {
                child.QueueFree();
            }
        }

        public IEnumerable<T> GetChildrenOfType<T>() where T : Node => node.GetChildren().OfType<T>();

        public T? GetFirstChildOfType<T>() where T : Node => node.GetChildrenOfType<T>().FirstOrDefault();

        public T AddImmediateChild<T>() where T : Node, new()
        {
            var child = new T();
            node.AddChild(child);
            return child;
        }

        public T GetOrAddImmediateChild<T>() where T : Node, new()
        {
            var child = node.GetFirstChildOfType<T>();
            child ??= node.AddImmediateChild<T>();
            return child;
        }
    }

    extension(Area2D area2D)
    {
        public CollisionShape2D CollisionShape => area2D.GetFirstChildOfType<CollisionShape2D>()!;
    }

    public static void SetStyleBox(this Panel panel, StyleBox styleBox)
    {
        const string panelStylePath = "theme_override_styles/panel";
        panel.Set(panelStylePath, styleBox);
    }

    public static void SetModulateAlpha(this CanvasItem canvasItem, float alpha)
    {
        canvasItem.Modulate = canvasItem.Modulate with { A = alpha.Clamp01() };
    }
}