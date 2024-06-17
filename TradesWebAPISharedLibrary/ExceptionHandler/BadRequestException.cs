using System.Net;

namespace TradesWebAPISharedLibrary.ExceptionHandler
{
    [Serializable]
    public class BadRequestException : Exception
    {
        public int StatusCode { get; }
        public BadRequestException()
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
        }

        public BadRequestException(string? message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;

        }
    }
}
