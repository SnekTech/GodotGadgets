namespace GodotGadgets.Tasks;

public static class TaskCancellationExtensions
{
    extension(CancellationTokenSource cancellationTokenSource)
    {
        /// <summary>
        /// 取消并释放。幂等: 对已经释放过的实例重复调用不会抛异常。
        /// (CancellationTokenSource 没有公开的 IsDisposed, 只能吞掉 Cancel 的 ODE;
        ///  Dispose 本身可重复调用。) 这样"取消 + 释放"就可以在任何地方安全地重复执行。
        /// </summary>
        public void CancelAndDispose()
        {
            try
            {
                cancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // 已经释放过: 取消已无从谈起, 忽略即可
                return;
            }

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
