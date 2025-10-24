using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public class PagedList<T> where T : class
    {
        public PagedList(int pageNumber, int pageSize, int totalCount, List<T> items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasNext { get => PageNumber * PageSize < TotalCount; }
        public bool HasPrevious { get => PageNumber > 0; }

        public List<T> Items { get; set; } = [];
    }
}
