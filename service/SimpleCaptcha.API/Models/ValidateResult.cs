using System.Net;

namespace SimpleCaptcha.API.Models
{
    public class ValidateResult
    {
        public bool IsValid { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
