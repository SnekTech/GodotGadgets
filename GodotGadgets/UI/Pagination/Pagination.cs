using GodotGadgets.Tasks;

namespace GodotGadgets.UI.Pagination;

public interface IPageQuery<TItem>
{
    Task<PageResult<TItem>> FetchPageAsync(PageRequest request, CancellationToken ct = default);
}

public record struct PageRequest(int PageIndex, int PageSize);
public record PageResult<TItem>(IReadOnlyList<TItem> Items, int TotalItemCount);

public sealed class Pagination<TItem>(IPageQuery<TItem> query, int pageSize = 10) : IDisposable
{
    readonly IPageQuery<TItem> _query = query ?? throw new ArgumentNullException(nameof(query));
    CancellationTokenSource? _navigationCts;
    readonly SemaphoreSlim _navLock = new(1, 1);

    public int PageSize { get; } = pageSize > 0 ? pageSize : throw new ArgumentOutOfRangeException(nameof(pageSize));

    public int CurrentPageIndex { get; private set; }

    public IReadOnlyList<TItem> CurrentItems { get; private set; } = Array.Empty<TItem>();

    public int TotalItemCount { get; private set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalItemCount / PageSize);

    // 导航能力
    public bool HasFirstPage => CurrentPageIndex > 0;
    public bool HasPreviousPage => CurrentPageIndex > 0;
    public bool HasNextPage => CurrentPageIndex < TotalPages - 1;
    public bool HasLastPage => CurrentPageIndex < TotalPages - 1;

    /// <summary>
    /// 当数据或页索引发生变更时引发。
    /// UI 应订阅此事件以刷新显示。
    /// </summary>
    public event Action? DataChanged;


    public Task LoadInitialAsync(CancellationToken ct = default) => GoToPageAsync(0, ct);

    public Task GoToFirstPageAsync(CancellationToken ct = default) => GoToPageAsync(0, ct);
    public Task GoToLastPageAsync(CancellationToken ct = default) => GoToPageAsync(TotalPages - 1, ct);
    public Task GoToPreviousPageAsync(CancellationToken ct = default) => HasPreviousPage ? GoToPageAsync(CurrentPageIndex - 1, ct) : Task.CompletedTask;
    public Task GoToNextPageAsync(CancellationToken ct = default) => HasNextPage ? GoToPageAsync(CurrentPageIndex + 1, ct) : Task.CompletedTask;

    public async Task GoToPageAsync(int pageIndex, CancellationToken ct = default)
    {
        _navigationCts?.CancelAndDispose();
        var linkedCts = ct.CreateLinked();
        _navigationCts = linkedCts;

        try
        {
            await _navLock.WaitAsync(_navigationCts.Token);
        }
        catch (OperationCanceledException)
        {
            // 被新请求取消了，没拿到锁，所以不用release
            return;
        }
        
        try
        {
            // 将 pageIndex 限制在有效范围 [0, TotalPages - 1]
            // 如果 TotalPages 为 0（数据为空），Clamp 会将任何值限制为 0，
            // 但实际我们会发送 pageIndex=0 的请求，让查询返回空结果。
            pageIndex = Math.Clamp(pageIndex, 0, Math.Max(0, TotalPages - 1));
            var request = new PageRequest(pageIndex, PageSize);
            var result = await _query.FetchPageAsync(request, linkedCts.Token);

            CurrentItems = result.Items;
            TotalItemCount = result.TotalItemCount;
            CurrentPageIndex = pageIndex;

            DataChanged?.Invoke();
        }
        finally
        {
            _navLock.Release();
        }
    }

    public void Dispose()
    {
        _navigationCts?.CancelAndDispose();
        _navLock.Dispose();
    }
}