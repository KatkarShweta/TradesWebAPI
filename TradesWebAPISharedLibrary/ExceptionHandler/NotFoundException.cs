using System.Net;

namespace TradesWebAPISharedLibrary.ExceptionHandler
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public int StatusCode { get; }
        public NotFoundException()
        {
            StatusCode = (int)HttpStatusCode.NotFound;
        }

        public NotFoundException(string? message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
