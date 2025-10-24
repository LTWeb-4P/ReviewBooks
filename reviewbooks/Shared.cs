namespace Shared
{
    public class PageResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public static PageResult<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var totalCount = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };
        }
    }


    public class Query
    {
        public string? search { get; set; }
        public string? sortBy { get; set; }
        public string? filterBy { get; set; }
        public bool isDescending { get; set; } = false;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public int SkipCount => (pageNumber - 1) * pageSize;

        public void Normalize()
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
        }

    }


    public static class QueryableExtensions
    {
        public static async Task<PageResult<T>> ToPageResultAsync<T>(
            this IQueryable<T> query,
            Query queryParams)
        {
            queryParams.Normalize(); // đảm bảo hợp lệ
            var totalCount = await Task.FromResult(query.Count());

            var items = query
                .Skip((queryParams.pageNumber - 1) * queryParams.pageSize)
                .Take(queryParams.pageSize)
                .ToList();

            return new PageResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = queryParams.pageSize,
                CurrentPage = queryParams.pageNumber
            };
        }
    }
}