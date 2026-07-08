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
        public CancellationTokenSource CreateLinked() => CancellationTokenSource.CreateLinkedTokenSource(token);
        public CancellationTokenSource LinkTo(CancellationToken anotherToken) =>
            CancellationTokenSource.CreateLinkedTokenSource(token, anotherToken);

        public CancellationTokenSource LinkWithNodeDestroy(Node node)
            => token.LinkTo(node.GetCancellationTokenOnTreeExit());
    }

    public static CancellationToken GetCancellationTokenOnTreeExit(this Node node)
    {
        if (!node.IsInsideTree())
            return new CancellationToken(true);

        var cts = new CancellationTokenSource();
        node.TreeExited += OnExit;
        return cts.Token;

        void OnExit()
        {
            cts.CancelAndDispose();
            node.TreeExited -= OnExit;
        }
    }
}