namespace GodotGadgets.Tasks;

public static class TaskCancellationExtensions
{
    extension(CancellationTokenSource cancellationTokenSource)
    {
        public void CancelAndDispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }

    extension(CancellationToken token)
    {
        public CancellationTokenSource LinkTo(CancellationToken anotherToken) =>
            CancellationTokenSource.CreateLinkedTokenSource(token, anotherToken);

        public CancellationTokenSource LinkWithNodeDestroy(Node node)
            => token.LinkTo(node.GetCancellationTokenOnTreeExit());
    }

    static readonly Dictionary<Node, CancellationTokenSource> TreeExitCtsCache = [];

    public static CancellationToken GetCancellationTokenOnTreeExit(this Node node)
    {
        return GetCts().Token;

        CancellationTokenSource GetCts()
        {
            if (TreeExitCtsCache.TryGetValue(node, out var cts))
                return cts;

            cts = new CancellationTokenSource();
            TreeExitCtsCache[node] = cts;

            node.TreeExiting += OnTreeExiting;
            node.TreeExited += OnTreeExited;

            return cts;

            void OnTreeExiting()
            {
                cts.Cancel();
            }

            void OnTreeExited()
            {
                cts.Dispose();
                TreeExitCtsCache.Remove(node);

                node.TreeExiting -= OnTreeExiting;
                node.TreeExited -= OnTreeExited;
            }
        }
    }
}