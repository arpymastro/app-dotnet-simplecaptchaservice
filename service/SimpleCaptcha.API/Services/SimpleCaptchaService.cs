using Microsoft.Extensions.Logging;
using SimpleCaptcha.API.Interfaces;
using SimpleCaptcha.API.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SimpleCaptcha.API.Services
{
    public class SimpleCaptchaService : IAmSimpleCaptchaService
    {
        #region private variables
        private readonly string _moduleName = "SimpleCaptchaService";
        private readonly ILogger<SimpleCaptchaService> _logger;

        private readonly Random _random = new Random();
        #endregion

        #region constructor
        public SimpleCaptchaService(ILogger<SimpleCaptchaService> logger) => _logger = logger;
        #endregion

        #region public method

        /// <summary>
        /// GenerateCaptcha() method helps to generate captcha of length 6
        /// </summary>
        /// <returns></returns>
        public CaptchaResult GenerateCaptcha()
        {
            var methodName = "GenerateCaptcha";
            CaptchaResult result = null;

            int imageWidth = 100;
            int imageHeight = 30;

            try
            {
                _logger.LogInformation($"[MethodStart]:{_moduleName}-{methodName}");

                var captchaCode = GetCaptchaCode();

                using (Bitmap bitmap = new Bitmap(imageWidth, imageHeight))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    RectangleF rectf = new RectangleF(5, 5, 0, 0);

                    int fontSize = imageWidth / captchaCode.Length;

                    graphics.Clear(Color.White);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawString(captchaCode, new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Italic, GraphicsUnit.Pixel), Brushes.Chocolate, rectf);
                    graphics.DrawRectangle(new Pen(Color.Blue), 1, 1, imageWidth - 3, imageHeight - 3);
                    graphics.Flush();

                    MemoryStream memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, ImageFormat.Png);

                    result = new CaptchaResult()
                    {
                        CaptchaCode = captchaCode,
                        CaptchaByteStream = memoryStream.ToArray(),
                        Timestamp = DateTime.Now
                    };
                }

                _logger.LogInformation($"[MethodEnd]:{_moduleName}-{methodName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Error]:{_moduleName}-{methodName}", ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// ValidateCaptcha() method is used for validating the user input and actual captcha code
        /// </summary>
        /// <param name="actualCaptchaCode"></param>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public bool ValidateCaptcha(string actualCaptchaCode, string userInput)
        {
            var methodName = "ValidateCaptcha";
            var isValid = false;
            try
            {
                _logger.LogInformation($"[MethodStart]:{_moduleName}-{methodName}");

                if (string.IsNullOrWhiteSpace(actualCaptchaCode) || string.IsNullOrWhiteSpace(userInput))
                    throw new ArgumentNullException();

                if (actualCaptchaCode.Equals(userInput))
                    isValid = true;

                _logger.LogInformation($"[MethodEnd]:{_moduleName}-{methodName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Error]:{_moduleName}-{methodName}", ex);
                throw;
            }

            return isValid;
        }
        

        #endregion

        #region private variable
        private string GetCaptchaCode()
        {
            string letters = "12346789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int captchaLength = 6;

            int maxRandom = letters.Length - 1;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < captchaLength; i++)
                stringBuilder.Append(letters[_random.Next(maxRandom)]);

            return stringBuilder.ToString();
        }
        #endregion

    }
}
