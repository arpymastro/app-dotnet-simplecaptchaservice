using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebMvcApp.Models;

namespace WebMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        private readonly string sessionCaptchaKey = "CaptchaCode";
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login login) 
        {
            var captchaCode = HttpContext.Session.GetString(sessionCaptchaKey);

            var captchaRequest = new CaptchaRequest() 
            {
                CaptchaCode = captchaCode,
                UserInput = login.CaptchaText
            };

            var jsonRequest = JsonConvert.SerializeObject(captchaRequest);
            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/api/captcha/validate");
            request.Content = stringContent;

            var client = _clientFactory.CreateClient();

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
                return View("Index");
            else
                ViewBag.InvalidCaptcha = true;

            return View();
        }

        public IActionResult Index() 
            => View();

        [Route("getcaptchaimage")]
        public IActionResult GetCaptchaImage()
        {
            CaptchaResult captchaResult;
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/captcha/generate");
            var client = _clientFactory.CreateClient();

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                captchaResult = JsonConvert.DeserializeObject<CaptchaResult>(result);

                HttpContext.Session.SetString(sessionCaptchaKey, captchaResult.CaptchaCode);

                Stream s = new MemoryStream(captchaResult.CaptchaByteStream);
                return new FileStreamResult(s, "image/png");
            }

            return null;
        }
    }
}
