using Microsoft.Extensions.Logging;
using Moq;
using SimpleCaptcha.API.Models;
using SimpleCaptcha.API.Services;
using System;
using Xunit;

namespace SimpleCaptcha.API.Tests
{
    public class SimpleCaptchaServiceTests
    {
        [Fact]
        public void GenerateCaptcha_Success()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SimpleCaptchaService>>();
            SimpleCaptchaService service = new SimpleCaptchaService(mockLogger.Object);

            // Act
            var captchaResult = service.GenerateCaptcha();

            // Assert
            Assert.NotNull(captchaResult);
            Assert.IsType<CaptchaResult>(captchaResult);
        }

        [Theory]
        [InlineData("xxx1ey", "xxx1ey", true)]
        [InlineData("xxx2ey", "xxx1ey", false)]
        public void ValidateCaptcha(string actualCaptchaCode, string userInput, bool expected)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SimpleCaptchaService>>();
            SimpleCaptchaService service = new SimpleCaptchaService(mockLogger.Object);

            // Act
            bool actual = service.ValidateCaptcha(actualCaptchaCode, userInput);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ValidateCaptcha_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SimpleCaptchaService>>();
            SimpleCaptchaService service = new SimpleCaptchaService(mockLogger.Object);

            // Act
            Action action = () => service.ValidateCaptcha("", "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("Value cannot be null.", exception.Message);
        }
    }
}
