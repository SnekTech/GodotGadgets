namespace GodotGadgets.UI.Pagination;

/// <summary>
/// 要取哪一页。页码/页大小的合法性由 <see cref="PaginationState"/> 保证（它的每个写入口都重建不变量），
/// 所以这里只是一对数据，不做校验。
/// </summary>
public readonly record struct PageRequest(int PageIndex, int PageSize);
