using SimpleCaptcha.API.Models;

namespace SimpleCaptcha.API.Interfaces
{
    public interface IAmSimpleCaptchaService
    {
        CaptchaResult GenerateCaptcha();
        bool ValidateCaptcha(string actualCaptchaCode, string userInput);
    }
}
