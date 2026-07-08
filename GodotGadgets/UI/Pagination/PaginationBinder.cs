using GodotGadgets.Tasks;

namespace GodotGadgets.UI.Pagination;

public interface IPaginationUI
{
    // 导航事件
    event Action? FirstPageRequested;
    event Action? PreviousPageRequested;
    event Action? NextPageRequested;
    event Action? LastPageRequested;

    // 页码显示
    void SetPageText(int currentPage, int totalPages);

    // 导航按钮的启用/禁用状态
    void SetNavigationEnabled(bool canGoFirst, bool canGoPrevious, bool canGoNext, bool canGoLast);

    // 内容区域管理
    void ClearContent();
    void AddContentItem(Control item);
}

public interface IAsyncContent<in TData>
{
    Task InitAsync(TData data, CancellationToken ct = default);
}

public sealed class PaginationBinder<TItem> : IDisposable
{
    readonly IPaginationUI _ui;
    readonly Pagination<TItem> _pagination;
    readonly Func<TItem, Control> _entryFactory;
    readonly Action<Exception>? _onError;

    CancellationTokenSource? _contentCts;
    readonly List<Task> _pendingContentTasks = [];

    // 保存委托引用，以便移除事件
    readonly Action _onPrev;
    readonly Action _onNext;
    readonly Action _onFirst;
    readonly Action _onLast;
    readonly Action _onDataChanged;

    public PaginationBinder(
        IPaginationUI ui,
        Pagination<TItem> pagination,
        Func<TItem, Control> entryFactory,
        Action<Exception>? onError = null)
    {
        _ui = ui;
        _pagination = pagination;
        _entryFactory = entryFactory;
        _onError = onError;

        _onPrev = () => SafeFireAndForget(pagination.GoToPreviousPageAsync());
        _onNext = () => SafeFireAndForget(pagination.GoToNextPageAsync());
        _onFirst = () => SafeFireAndForget(pagination.GoToFirstPageAsync());
        _onLast = () => SafeFireAndForget(pagination.GoToLastPageAsync());
        _onDataChanged = () => SafeFireAndForget(RefreshAsync());

        ui.PreviousPageRequested += _onPrev;
        ui.NextPageRequested += _onNext;
        ui.FirstPageRequested += _onFirst;
        ui.LastPageRequested += _onLast;

        _pagination.DataChanged += _onDataChanged;

        // 初始加载
        SafeFireAndForget(pagination.LoadInitialAsync());
    }

    // todo: use extension method for fire & forget
    async void SafeFireAndForget(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // normal cancellation, ignore
        }
        catch (Exception ex)
        {
            if (_onError != null)
                _onError(ex);
            else
                System.Diagnostics.Debug.WriteLine($"[PaginationBinder] Unhandled: {ex}");
        }
    }

    async Task RefreshAsync()
    {
        _contentCts?.CancelAndDispose();
        _contentCts = new CancellationTokenSource();

        if (_pendingContentTasks.Count > 0)
        {
            try
            {
                await Task.WhenAll(_pendingContentTasks);
            }
            catch (OperationCanceledException)
            {
            }

            _pendingContentTasks.Clear();
        }

        UpdateNavigationState();
        UpdatePageText();
        UpdateContent();

        return;

        void UpdateNavigationState()
        {
            _ui.SetNavigationEnabled(
                _pagination.HasFirstPage,
                _pagination.HasPreviousPage,
                _pagination.HasNextPage,
                _pagination.HasLastPage);
        }

        void UpdateContent()
        {
            _ui.ClearContent();
            foreach (var item in _pagination.CurrentItems)
            {
                var control = _entryFactory(item);
                _ui.AddContentItem(control);

                if (control is IAsyncContent<TItem> asyncContent)
                {
                    var task = asyncContent.InitAsync(item, _contentCts.Token);
                    _pendingContentTasks.Add(task);
                }
            }
        }

        void UpdatePageText() => _ui.SetPageText(_pagination.CurrentPageIndex + 1, _pagination.TotalPages);
    }

    public void Dispose()
    {
        _contentCts?.CancelAndDispose();
        _pagination.Dispose();

        _ui.PreviousPageRequested -= _onPrev;
        _ui.NextPageRequested -= _onNext;
        _ui.FirstPageRequested -= _onFirst;
        _ui.LastPageRequested -= _onLast;
        _pagination.DataChanged -= _onDataChanged;
    }
}