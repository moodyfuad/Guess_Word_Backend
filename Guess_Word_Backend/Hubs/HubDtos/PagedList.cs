namespace Guess_Word_Backend.Hubs.HubDtos
{
    public class PagedList <T> where T : class
    {
        public PagedList(int pageNumber, int pageSize, int totalCount, List<T> items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }

        public int PageNumber { get; set; }
        public int PageSize {get;set;}
        public int TotalCount {get;set;}
        public bool HasNext {get => PageNumber * PageSize < TotalCount;}
        public bool HasPrevious {get => PageNumber > 0;}

        protected List<T> Items { get; set; } = [];
    }
}
