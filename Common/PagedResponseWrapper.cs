using cahrdipos_system.Models;
using System.Net;

namespace cahrdipos_system.Common
{
    public class PagedResponseWrapper<T> : ResponseWrapper<PagedResponse<T>>
    {
        public PagedResponseWrapper(PagedResponse<T> data, HttpStatusCode status, string message)
            : base(data, status, message)
        {
        }
    }
}
