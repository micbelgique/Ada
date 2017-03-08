using System.Net;

namespace AdaSDK
{
    public class WebServiceError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
    }
}
