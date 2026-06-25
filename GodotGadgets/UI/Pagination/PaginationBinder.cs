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

    CancellationTokenSource? _refreshCts;

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

        // 创建无捕获的委托，以便正确移除
        _onPrev = () => SafeFireAndForget(ct => _pagination.GoToPreviousPageAsync(ct));
        _onNext = () => SafeFireAndForget(ct => _pagination.GoToNextPageAsync(ct));
        _onFirst = () => SafeFireAndForget(ct => _pagination.GoToFirstPageAsync(ct));
        _onLast = () => SafeFireAndForget(ct => _pagination.GoToLastPageAsync(ct));
        _onDataChanged = () => SafeFireAndForget(_ => RefreshAsync());

        ui.PreviousPageRequested += _onPrev;
        ui.NextPageRequested += _onNext;
        ui.FirstPageRequested += _onFirst;
        ui.LastPageRequested += _onLast;

        _pagination.DataChanged += _onDataChanged;

        // 初始加载
        SafeFireAndForget(ct => _pagination.LoadInitialAsync(ct));
    }
    
    async void SafeFireAndForget(Func<CancellationToken, Task> asyncAction)
    {
        try
        {
            await asyncAction(_refreshCts?.Token ?? CancellationToken.None);
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

    Task RefreshAsync()
    {
        _refreshCts?.CancelAndDispose();
        _refreshCts = new CancellationTokenSource();
        
        UpdateNavigationState();
        UpdatePageText();
        UpdateContent();

        return Task.CompletedTask;

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
                    SafeFireAndForget(ct => asyncContent.InitAsync(item, ct));
                }
            }
        }

        void UpdatePageText() => _ui.SetPageText(_pagination.CurrentPageIndex + 1, _pagination.TotalPages);
    }

    public void Dispose()
    {
        _refreshCts?.CancelAndDispose();
        
        _ui.PreviousPageRequested -= _onPrev;
        _ui.NextPageRequested -= _onNext;
        _ui.FirstPageRequested -= _onFirst;
        _ui.LastPageRequested -= _onLast;
        _pagination.DataChanged -= _onDataChanged;
    }
}