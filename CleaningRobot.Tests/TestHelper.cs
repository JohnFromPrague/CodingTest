using CleaningRobot.Services;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleaningRobot.Tests
{
    internal static class TestHelper
    {

        internal static void VerifyLogMessage<T>(Mock<ILogger<T>> logger, string logMessage, LogLevel logLevel)
        {
            logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == logLevel),
                    0,
                    It.Is<It.IsAnyType>((@o, @t) => @o.ToString().StartsWith(logMessage) && @t.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}