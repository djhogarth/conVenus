using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParameters
    {
      private const int MaxPageSize = 50;

        public int PageNumber {get; set;} = 1;

        //Default Page size
        private int _pageSize = 10;

        public int PageSize
        {
          //only allow a max page size of 50.
          get => _pageSize;
          set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

    }
}
