using System.Collections.Generic;

namespace ThreeOldFloor.Entity.Api
{
    public class PaginationBase<T> where T:new()
    {
        public int PageSize { get; set; }

        public int TotalSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
