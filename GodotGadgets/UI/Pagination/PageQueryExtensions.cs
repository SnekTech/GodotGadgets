namespace GodotGadgets.UI.Pagination;

public static class PageQueryExtensions
{
    extension<T>(IReadOnlyList<T> source)
    {
        public PageResult<T> SlicePage(PageRequest request)
        {
            var (pageIndex, pageSize) = request;
            var itemsInPage = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToArray();
            return new PageResult<T>(itemsInPage, source.Count);
        }
    }

    extension<T>(IEnumerable<T> source)
    {
        public PageResult<T> SlicePage(PageRequest request)
        {
            var materialized = source.ToArray();
            return materialized.SlicePage(request);
        }
    }
}