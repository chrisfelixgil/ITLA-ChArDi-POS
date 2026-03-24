using System.Net;

namespace cahrdipos_system.Common
{
    public class ResponseWrapper<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
        public T? Data { get; set; }

        public ResponseWrapper() { }

        public ResponseWrapper(T data, HttpStatusCode httpStatus, string message = "")
        {
            Success = true;
            Message = message;
            HttpStatus = httpStatus;
            Data = data;
        }

        public ResponseWrapper(string errorMessege, HttpStatusCode httpStatus)
        {
            Success = false;
            Message = errorMessege;
            HttpStatus = httpStatus;
            Data = default;
        }


    }
}
