using System.Net;

namespace AdaBridge
{
    public class WebServiceError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
    }
}
