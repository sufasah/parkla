namespace Parkla.Web.Models;
public  class PageDto
{
    const int maxPageSize = 50;
    public int NextRecord { get; set; } = 0;
    private int _pageSize = 10;
    public int PageSize { 
        get {
            return _pageSize;
        }  
        set {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
            _pageSize = value < 1 ? 10 : value;
        }
    }

}