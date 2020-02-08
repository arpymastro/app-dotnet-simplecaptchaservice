namespace WebMvcApp.Models
{
    public class CaptchaRequest
    {
        public string CaptchaCode { get; set; }

        public string UserInput { get; set; }
    }
}
