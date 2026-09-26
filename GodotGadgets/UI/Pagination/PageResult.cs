namespace GodotGadgets.UI.Pagination;

/// <summary>
/// 一次取数的答案：**窗口**（本页的条目）+ 它成立所依赖的**边界**（总数）。
/// <para>
/// 不变量：<see cref="Items"/> 与 <see cref="TotalItemCount"/> 必须来自**同一份快照**。
/// 分页器用总数算总页数、并用它把页码归一化（见 <c>PaginationState.WithTotalItemCount</c>），
/// 两者若取自不同时刻会得到自相矛盾的状态。内存源天然满足（<c>SlicePage</c> 用的是同一个数组）；
/// 远端实现必须自己保证（例如在同一次查询里同时取窗口与计数）。
/// </para>
/// </summary>
public record PageResult<TItem>(IReadOnlyList<TItem> Items, int TotalItemCount);
