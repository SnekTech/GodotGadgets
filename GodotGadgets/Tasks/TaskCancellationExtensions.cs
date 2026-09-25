namespace GodotGadgets.Tasks;

public static class TaskCancellationExtensions
{
    // key 用 InstanceId: 不持有节点引用(字典不该决定节点生死)
    static readonly Dictionary<ulong, CancellationTokenSource> NodeExitTokens = [];

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

    /// <summary>
    /// 节点退树即取消的 token。<b>每个节点一份, 按需创建后缓存</b> ——
    /// 旧实现是"每次调用都新建一个 CTS + 在 node.TreeExited 上挂一个回调", 于是在按次调用的热路径上
    /// (每次玩家输入 / 每次 tween / 每条消息) 该节点的订阅列表会随调用次数单调增长; 缓存版降为一次字典查找。
    /// <para>
    /// 三条不能丢的规矩: (1) 调用方只拿到 token、拿不到 source ⇒ 只有"退树"能取消它, 不会被误 cancel/dispose;
    /// (2) 退树时必须同时移除缓存条目, 否则换成另一种泄漏(字典无界增长);
    /// (3) key 用 InstanceId 而不是 Node 引用 —— 后者会让字典持有节点, 反而阻止节点释放。
    /// </para>
    /// 只在主线程调用(库内既有约定), 因此不加锁。
    /// </summary>
    public static CancellationToken GetCancellationTokenOnTreeExit(this Node node)
    {
        if (!node.IsInsideTree())
            return new CancellationToken(true);

        var instanceId = node.GetInstanceId();
        if (NodeExitTokens.TryGetValue(instanceId, out var cached))
            return cached.Token;

        var cts = new CancellationTokenSource();
        NodeExitTokens.Add(instanceId, cts);
        node.TreeExited += OnTreeExited;
        return cts.Token;

        void OnTreeExited()
        {
            // 节点可能被 RemoveChild 后重新 AddChild(同一实例还活着): 必须退订, 否则它下次退树会去取消
            // "重新入树后新建的那份 token"
            node.TreeExited -= OnTreeExited;
            NodeExitTokens.Remove(instanceId);
            cts.CancelAndDispose();
        }
    }
}
