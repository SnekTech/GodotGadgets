using System.Runtime.CompilerServices;

namespace GodotGadgets.UI.Pagination;

/// <summary>
/// 翻页状态的唯一真相：当前页码 + 每页条数 + 总条数。
/// <para>
/// 不变量：<c>0 &lt;= PageIndex &lt;= max(0, TotalPages - 1)</c>。
/// 两个写入口（<see cref="Go"/> / <see cref="WithTotalItemCount"/>）都会重新建立它，
/// 所以外部拿到状态后直接读即可，不需要（也不应该）再自己 clamp。
/// </para>
/// <para>
/// 这里没有"加载中 / 未加载"这个维度：取数不属于核心（见 <c>PaginationBinder</c>）。
/// 也正因为如此，"没取过" 与 "取过但为空" 都是 <c>TotalItemCount == 0</c> —— 核心同步执行时两者没有可观察差别。
/// </para>
/// </summary>
public readonly record struct PaginationState(int PageIndex, int PageSize, int TotalItemCount)
{
    /// <summary>未取数时的初始状态（第 0 页、总数 0）。</summary>
    public static PaginationState Initial(int pageSize) => pageSize > 0
        ? new PaginationState(0, pageSize, 0)
        : throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "page size must be positive");

    /// <summary>总数 0（含 <c>default</c> 下的 PageSize 0）时是 0 页，而不是除零后的垃圾值。</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItemCount / PageSize);

    /// <summary>还能不能往前/往后走。UI 的"首页"按钮与"上一页"共用前者，"末页"与"下一页"共用后者。</summary>
    public bool HasPreviousPage => PageIndex > 0;

    public bool HasNextPage => PageIndex < TotalPages - 1;

    /// <summary>照当前状态该取哪一页 —— 页码已规整，可以直接拿去查。</summary>
    public PageRequest CurrentRequest => new(PageIndex, PageSize);

    /// <summary>走到 <paramref name="nav"/> 想去的那一页；已经在那一页（或被规整回去）时原样返回。</summary>
    public PaginationState Go(PageNav nav) =>
        WithPageIndex(nav switch
        {
            PageNav.First => 0,
            PageNav.Previous => PageIndex - 1,
            PageNav.Next => PageIndex + 1,
            PageNav.Last => TotalPages - 1,
            PageNav.To to => to.PageIndex,
            _ => throw new SwitchExpressionException(),
        });

    /// <summary>
    /// 写入数据源的当前总条数（即取数结果里的 <c>PageResult.TotalItemCount</c>）。
    /// 总数是页码的**依赖字段**：若它变小导致当前页越界，这里顺手把页码归一化回合法范围
    /// （否则会得到 <c>PageIndex = 4</c> / <c>TotalPages = 2</c> 这类自相矛盾的状态，UI 会显示成 "5 / 2"）。
    /// </summary>
    public PaginationState WithTotalItemCount(int totalItemCount) =>
        (this with { TotalItemCount = totalItemCount }).WithPageIndex(PageIndex);

    PaginationState WithPageIndex(int pageIndex) =>
        this with { PageIndex = Math.Clamp(pageIndex, 0, Math.Max(0, TotalPages - 1)) };

    /// <summary>投影给视图的那几个值（页码转成 1-based，避免每个视图都自己 +1）。</summary>
    public PaginationViewData ToViewData() =>
        new(PageNumber: PageIndex + 1, TotalPages: TotalPages, HasPreviousPage: HasPreviousPage, HasNextPage: HasNextPage);
}

/// <summary>
/// 视图要显示的全部内容：页码文字的两个数与导航按钮的两个位。
/// <para>
/// 只有两个位是领域真值（<b>首页/末页</b>按钮分别与<b>上一页/下一页</b>共用同一位 —— 在首页就不存在"上一页"，
/// 这是定义上的恒等，不是巧合），所以不在这里重复成四个 bool。
/// </para>
/// </summary>
public readonly record struct PaginationViewData(int PageNumber, int TotalPages, bool HasPreviousPage,
    bool HasNextPage);
