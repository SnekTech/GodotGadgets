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

        public T GetFirstChildOfType<T>() where T : Node => node.GetChildrenOfType<T>().First();
    }

    extension(Area2D area2D)
    {
        public CollisionShape2D CollisionShape => area2D.GetFirstChildOfType<CollisionShape2D>();
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

    private static readonly Dictionary<Node, CancellationTokenSource> TreeExitCtsDict = [];

    public static CancellationToken GetCancellationTokenOnTreeExit(this Node node)
    {
        return GetCts().Token;

        CancellationTokenSource GetCts()
        {
            if (!TreeExitCtsDict.TryGetValue(node, out var cts))
            {
                cts = new CancellationTokenSource();
                TreeExitCtsDict[node] = cts;

                node.TreeExiting += OnTreeExiting;
                node.TreeExited += OnTreeExited;
            }

            return cts;

            void OnTreeExiting()
            {
                cts.Cancel();
            }

            void OnTreeExited()
            {
                cts.Dispose();
                TreeExitCtsDict.Remove(node);

                node.TreeExiting -= OnTreeExiting;
                node.TreeExited -= OnTreeExited;
            }
        }
    }
}