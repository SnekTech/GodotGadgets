namespace GodotGadgets.UI.Pagination;

/// <summary>
/// 翻页视图契约（Godot-aware 的那一侧）：只负责"把页面画出来"，不做任何决策。
/// <para>
/// 两个"一次给全"是刻意的：
/// <list type="bullet">
/// <item>页码与导航可用性是**同一个快照的两面**，分两次调用会出现"文字已更新、按钮还没"的中间态；</item>
/// <item>内容是**整屏替换**，所以"先清后加"的协议不出现在接口上（空列表 = 清空）。</item>
/// </list>
/// </para>
/// </summary>
public interface IPaginationView
{
    /// <summary>用户按了某个导航按钮（首页 / 上一页 / 下一页 / 末页）。</summary>
    event Action<PageNav>? NavigationRequested;

    /// <summary>页码文字与导航可用性。</summary>
    void ShowPage(PaginationViewData page);

    /// <summary>整屏替换内容；<c>[]</c> 表示清空。</summary>
    void ShowItems(IReadOnlyList<Control> items);
}

/// <summary>
/// 内容条目在入树后还需要异步初始化时实现它（例如卡片要播一段入场动画）。
/// 由 binder 启动、fire-and-forget —— 所以 <see cref="InitAsync"/> 里 await 之后**不要**再碰节点，
/// 除非先检查与那棵树同命的 token（见 docs/prompts/await-node-lifetime-audit.md）。
/// </summary>
public interface IAsyncContent<in TData>
{
    Task InitAsync(TData data, CancellationToken ct = default);
}
