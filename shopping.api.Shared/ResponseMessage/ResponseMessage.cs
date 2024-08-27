using System.Collections.Generic;

namespace shopping.api.Shared.ResponseMessage
{
    public class ResponseMessage<T>
    {
        public string Message { get; set; }
        public int? LastPage { get; set; }
        public int? Total { get; set; }
        public T Result { get; set; }
        public IEnumerable<T> Results { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? TotalPage { get; set; }
    }

}