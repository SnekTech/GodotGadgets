namespace GodotGadgets.UI.Pagination;

/// <summary>
/// 一次导航请求：说的是"用户想做哪种导航"，而不是"结果页码"。
/// 结果页码由 <see cref="PaginationState.Go"/> 结合当前状态算出并规整 ——
/// 所以越界/非法输入在这里被吸收，UI 不需要先知道总页数。
/// case 嵌在类型里，避免 <c>First</c> / <c>Next</c> 这类很泛的名字污染 namespace
/// （与 <c>PointerTarget</c> 同风格）。
/// </summary>
public abstract record PageNav
{
    public sealed record First : PageNav;

    public sealed record Previous : PageNav;

    public sealed record Next : PageNav;

    public sealed record Last : PageNav;

    /// <summary>跳到指定页；超界会被规整进 <c>[0, TotalPages - 1]</c>。</summary>
    public sealed record To(int PageIndex) : PageNav;
}
