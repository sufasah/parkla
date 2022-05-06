namespace Parkla.Web.Helpers;
public class PagedList<T> : List<T>
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int TotalPages { get; set; }
    public bool HasPrevious => PageNumber > 1; 
    public bool HasNext => PageNumber < TotalPages; 
    public int PageSize { 
        get {
            return _pageSize;
        }  
        set {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
    
    public PagedList(List<T> items, int pageNumber, int pageSize, int totalPages)
    {
        TotalPages = totalPages;
        PageSize = pageSize;
        PageNumber = pageNumber;
        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize) {
        var count = source.Count();
        var items = source.Skip((pageNumber-1) * pageSize)
            .Take(pageSize)
            .ToList();
        return new PagedList<T>(items, pageNumber, pageSize, count);
    }
}