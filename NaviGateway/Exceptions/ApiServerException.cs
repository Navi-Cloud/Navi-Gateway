using System.Net;

namespace NaviGateway.Exceptions
{
    public class ApiServerException: System.Exception
    {
        private readonly HttpStatusCode _httpStatusCode;
        private readonly string _message;

        private readonly System.Exception _exception;

        public ApiServerException(HttpStatusCode statusCode, string message)
        {
            _httpStatusCode = statusCode;
            _message = message;
        }

        public ApiServerException(HttpStatusCode statusCode, string message, System.Exception exception)
        {
            _httpStatusCode = statusCode;
            _message = message;
            _exception = exception;
        }
    }
}