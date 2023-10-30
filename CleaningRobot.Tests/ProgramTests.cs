using CleaningRobot.Model;
using CleaningRobot.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using System.Text.Json;

namespace CleaningRobot.Tests
{
    public class ProgramTests
    {
        [Theory]
        [InlineData("test1.json", "test1_result.json")]
        [InlineData("test2.json", "test2_result.json")]
        public void Integration(string sourceFilePath, string expectedResultFilePath)
        {
            var testResourceFolder = "./Resources/";
            var actualResultFilePath = Path.GetTempFileName();

            var serviceProvider = Program.ConfigureServices().BuildServiceProvider();

            Program.Run(
                new[]
                {
                    Path.Combine(testResourceFolder, sourceFilePath),
                    actualResultFilePath
                },
                serviceProvider);

            Assert.True(Path.Exists(actualResultFilePath));

            var options = serviceProvider.GetRequiredService<JsonSerializerOptions>();
            var expectedSession = JsonSerializer.Deserialize<CleaningSession>(File.ReadAllText(Path.Combine(testResourceFolder, expectedResultFilePath)), options);
            var actualSession = JsonSerializer.Deserialize<CleaningSession>(File.ReadAllText(Path.Combine(testResourceFolder, actualResultFilePath)), options);

            Assert.Equal(JsonSerializer.Serialize(expectedSession, options), JsonSerializer.Serialize(actualSession, options));
        }

        [Fact]
        public void InvalidNumberOfParameters()
        {
            var logger = new Mock<ILogger<FileService>>();
            var services = ConfigureServices(logger);

            Program.Run(
                new[] { "C:\\xy.json" },
                services.BuildServiceProvider());
            TestHelper.VerifyLogMessage(logger, LogMessages.TwoParametersRequired, LogLevel.Error);
        }

        [Fact]
        public void InvalidNumberOfParameters2()
        {
            var logger = new Mock<ILogger<FileService>>();
            var services = ConfigureServices(logger);

            Program.Run(
                new[]
                {
                    "C:\\xy.json",
                    "C:\\xy.json",
                    "C:\\xy.json"
                },
                services.BuildServiceProvider());
            TestHelper.VerifyLogMessage(logger, LogMessages.TwoParametersRequired, LogLevel.Error);
        }

        [Fact]
        public void SourceFileDoesNotExist()
        {
            var logger = new Mock<ILogger<FileService>>();
            var services = ConfigureServices(logger);
            var sourceFilePath = "C:\\xy.json";

            Program.Run(
                new[]
                {
                    sourceFilePath,
                    "C:\\abc.json"
                },
                services.BuildServiceProvider());
            TestHelper.VerifyLogMessage(logger, string.Format(LogMessages.SourceFileNotFound, sourceFilePath), LogLevel.Error);
        }

        [Fact]
        public void ResultFilePathCannotBeEmpty()
        {
            var logger = new Mock<ILogger<FileService>>();
            var services = ConfigureServices(logger);
            var tempFile = Path.GetTempFileName();

            Program.Run(
                new[]
                {
                    tempFile,
                    "      "
                },
                services.BuildServiceProvider());
            TestHelper.VerifyLogMessage(logger, LogMessages.EmptyResultFilePath, LogLevel.Error);
        }

        [Fact]
        public void ResultFilePathCannotBeDirectory()
        {
            var logger = new Mock<ILogger<FileService>>();
            var services = ConfigureServices(logger);
            var tempFile = Path.GetTempFileName();
            var folder = "C:\\folder";

            Program.Run(
                new[]
                {
                    tempFile,
                    folder
                },
                services.BuildServiceProvider());
            TestHelper.VerifyLogMessage(logger, string.Format(LogMessages.InvalidResultFilePath, folder), LogLevel.Error);
        }

        private static ServiceCollection ConfigureServices(Mock<ILogger<FileService>> logger)
        {
            var services = Program.ConfigureServices();
            services.AddTransient(sp => logger.Object);
            return services;
        }
    }
}