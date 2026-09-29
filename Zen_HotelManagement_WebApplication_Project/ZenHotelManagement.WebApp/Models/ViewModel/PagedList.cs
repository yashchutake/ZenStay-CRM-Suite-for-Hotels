namespace ZenHotelManagement.WebApp.Models.ViewModel
{
    public class PagedList<T>
    {
        public List<T> Items { get; private set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public object? Filter { get; private set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize, object? filter = null)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Filter = filter;
        }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;        
        public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize, object? filter = null)
        {
            var count = source.Count();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            
            // Adjust pageNumber if it exceeds the total pages
            if (pageNumber > totalPages && totalPages > 0)
            {
                pageNumber = totalPages;
            }
            
            var items = source.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();
                             
            return new PagedList<T>(items, count, pageNumber, pageSize, filter);
        }
    }
}
