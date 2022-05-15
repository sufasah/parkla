namespace Parkla.Core.Helpers;
public class PagedList<T> : List<T>
{
    const int maxPageSize = 50;
    public int NextRecord { get; set; } = 0;
    private int _pageSize = 10;
    public int TotalRecords { get; set; }
    public bool HasPrevious => NextRecord >= _pageSize; 
    public bool HasNext => NextRecord <= TotalRecords - _pageSize; 
    public int PageSize { 
        get {
            return _pageSize;
        }  
        set {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
            _pageSize = (value < 1) ? 10 : value;
        }
    }
    
    public PagedList(List<T> items, int nextRecord, int pageSize, int count)
    {
        TotalRecords = count;
        PageSize = pageSize;
        NextRecord = nextRecord;
        AddRange(items);
    }
}