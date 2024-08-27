using System;
using shopping.api.Shared.Models;

namespace shopping.api.Shared.ResponseMessage
{
    public static class Page
    {
        /* offset is meaning index */
        public static Pagination Pagination(int offset, int limit, int count)
        {
            return new Pagination
            {
                Page = offset + 1,
                PageSize = limit,
                TotalPage = _TotalPage(count, limit)
            };
        }

        private static int _TotalPage(int count, int limit)
        {
            return (int)Math.Ceiling((float)count / (float)limit);
        }
    }

}