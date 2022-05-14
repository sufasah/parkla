namespace Parkla.Core.Helpers;
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
            _pageSize = (value < 1) ? 10 : value;
        }
    }
    
    public PagedList(List<T> items, int pageNumber, int pageSize, int count)
    {
        TotalPages = count == 0 ? 0 : count / pageSize;
        PageSize = pageSize;
        PageNumber = pageNumber;
        AddRange(items);
    }
}