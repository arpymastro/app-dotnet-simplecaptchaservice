using System;

namespace WebMvcApp.Models
{
    public class CaptchaResult
    {
        public string CaptchaCode { get; set; }
        public byte[] CaptchaByteStream { get; set; }
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteStream);
        public DateTime Timestamp { get; set; }
    }
}
