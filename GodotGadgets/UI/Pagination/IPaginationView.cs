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
/// 内容条目在入树后需要自己初始化时实现它（此时节点已在树里，可以安全地碰场景节点）。
/// 由 binder 在 <see cref="IPaginationView.ShowItems"/> 之后同步调用；需要异步的部分由条目自己 fire-and-forget。
/// </summary>
public interface IInitializableContent<in TData>
{
    void Init(TData data);
}
