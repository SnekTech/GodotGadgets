using GodotGadgets.Tasks;

namespace GodotGadgets.UI.Pagination;

/// <summary>
/// 把翻页状态接到视图上：导航事件 → 纯转移 → 取数 → 渲染。
/// <para>
/// 取数走 <see cref="Func{T,TResult}"/> 缝（今天两个数据源都是内存切片，同步即可）。
/// 这里**没有** async / CancellationToken / 锁 / 事件回灌 —— 因为核心不含异步，也就不存在"迟到的结果"。
/// 将来真接异步数据源时，再引入代号（generation）匹配即可，<see cref="PaginationState"/> 不需要变。
/// </para>
/// </summary>
public sealed class PaginationBinder<TItem> : IDisposable
{
    readonly IPaginationView _view;
    readonly Func<PageRequest, PageResult<TItem>> _fetchPage;
    readonly Func<TItem, Control> _entryFactory;

    PaginationState _state;

    public PaginationBinder(
        IPaginationView view,
        int pageSize,
        Func<PageRequest, PageResult<TItem>> fetchPage,
        Func<TItem, Control> entryFactory
    )
    {
        _view = view;
        _fetchPage = fetchPage;
        _entryFactory = entryFactory;
        _state = PaginationState.Initial(pageSize);

        _view.NavigationRequested += OnNavigationRequested;

        Render(); // 绑定即渲染第 0 页（等价于旧的 LoadInitialAsync）
    }

    void OnNavigationRequested(PageNav nav)
    {
        var next = _state.Go(nav);
        if (next == _state) return; // 首页按"上一页"之类：状态没变，也就不必重新取数

        _state = next;
        Render();
    }

    void Render()
    {
        var result = _fetchPage(_state.CurrentRequest);
        _state = _state.WithTotalItemCount(result.TotalItemCount);
        // 若归一化真的改了页码（取数期间总数缩水），本次显示的条目会对不上页码。
        // 今天数据在页面的生命周期内是稳定的，所以不处理；真要处理就是在这里重取一次。

        _view.ShowPage(_state.ToViewData());

        var items = result.Items;
        var controls = new Control[items.Count];
        for (var i = 0; i < items.Count; i++) controls[i] = _entryFactory(items[i]);
        _view.ShowItems(controls);

        // 条目自己可能还要异步初始化（卡片动画等）——交给它 fire-and-forget。
        // 这里刻意**不** await：await 之后再碰节点正是"续体撞上已销毁节点"的窗口来源
        // （旧实现在这里 await Task.WhenAll 然后写 _ui，就是审计里的 #3）。
        for (var i = 0; i < items.Count; i++)
        {
            if (controls[i] is IAsyncContent<TItem> asyncContent) asyncContent.InitAsync(items[i]).Fire();
        }
    }

    public void Dispose() => _view.NavigationRequested -= OnNavigationRequested;
}
