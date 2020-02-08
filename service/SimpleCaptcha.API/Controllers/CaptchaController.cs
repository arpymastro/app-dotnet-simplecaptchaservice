using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleCaptcha.API.Interfaces;
using SimpleCaptcha.API.Models;
using System.IO;
using System.Net;

namespace SimpleCaptcha.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        #region private variable
        private const string _moduleName = "CaptchaController";
        private readonly ILogger<CaptchaController> _logger;
        
        private readonly IAmSimpleCaptchaService _simpleCaptchaService;
        #endregion

        #region constructor
        public CaptchaController(ILogger<CaptchaController> logger, IAmSimpleCaptchaService simpleCaptchaService)
        {
            _logger = logger;
            _simpleCaptchaService = simpleCaptchaService;
        }
        #endregion

        #region public method
        [HttpGet]
        [Route("generate")]
        public IActionResult Get()
        {
            string methodName = "Get";
            try
            {
                _logger.LogInformation($"[MethodStart]:{_moduleName}-{methodName}");

                var captcha = _simpleCaptchaService.GenerateCaptcha();

                _logger.LogInformation($"[MethodEnd]:{_moduleName}-{methodName}");

                Stream s = new MemoryStream(captcha.CaptchaByteStream);
                //return new FileStreamResult(s, "image/png");

                return new ObjectResult(captcha) { StatusCode = (int)HttpStatusCode.OK };
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"[Error]:{_moduleName}-{methodName}", ex);
                return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        [HttpPost]
        [Route("validate")]
        public IActionResult Post([FromBody] CaptchaRequest captchaRequest)
        {
            string methodName = "Post";
            ValidateResult validateResult = null;

            try
            {
                _logger.LogInformation($"[MethodStart]:{_moduleName}-{methodName}");

                var isValid = _simpleCaptchaService.ValidateCaptcha(captchaRequest.CaptchaCode, captchaRequest.UserInput);

                _logger.LogInformation($"[MethodEnd]:{_moduleName}-{methodName}");

                if (isValid)
                    validateResult = new ValidateResult() { IsValid = isValid, HttpStatusCode = HttpStatusCode.OK };
                else
                    validateResult = new ValidateResult() { IsValid = isValid, HttpStatusCode = HttpStatusCode.BadRequest };

                return new ObjectResult(validateResult) { StatusCode = (int)validateResult.HttpStatusCode };
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"[Error]:{_moduleName}-{methodName}", ex);
                return new ObjectResult(ex.Message) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
        #endregion
    }
}