namespace SchoolHubAPI.Shared.RequestFeatures;

public class RequestParameters
{ 
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1; // Default page number
    private int _pageSize = 10; // Default page size

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }

    public string? OrderBy { get; set; } = "name";
    public string? SearchTerm { get; set; }
}
