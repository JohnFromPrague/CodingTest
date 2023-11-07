using CleaningRobot.Models;
using CleaningRobot.Models.Commands;
using CleaningRobot.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            var result = Program.Run(
                new[]
                {
                    Path.Combine(testResourceFolder, sourceFilePath),
                    actualResultFilePath
                },
                serviceProvider);

            Assert.Equal(0, result);
            Assert.True(Path.Exists(actualResultFilePath));

            var options = serviceProvider.GetRequiredService<JsonSerializerOptions>();
            var expectedSession = JsonSerializer.Deserialize<CleaningSession>(File.ReadAllText(Path.Combine(testResourceFolder, expectedResultFilePath)), options);
            var actualSession = JsonSerializer.Deserialize<CleaningSession>(File.ReadAllText(Path.Combine(testResourceFolder, actualResultFilePath)), options);

            Assert.Equal(JsonSerializer.Serialize(expectedSession, options), JsonSerializer.Serialize(actualSession, options));
        }

        [Fact]
        public void InvalidNumberOfParameters()
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);

            var result = Program.Run(
                new[] { "C:\\xy.json" },
                services.BuildServiceProvider());

            Assert.Equal(1, result);
            TestHelper.VerifyLogMessage(logger, LogMessages.TwoParametersRequired, LogLevel.Error);
            logger.VerifyNoOtherCalls();
        }

        [Fact]
        public void InvalidNumberOfParameters2()
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);

            var result = Program.Run(
                new[]
                {
                    "C:\\xy.json",
                    "C:\\xy.json",
                    "C:\\xy.json"
                },
                services.BuildServiceProvider());

            Assert.Equal(1, result);
            TestHelper.VerifyLogMessage(logger, LogMessages.TwoParametersRequired, LogLevel.Error);
            logger.VerifyNoOtherCalls();
        }

        [Fact]
        public void SourceFileDoesNotExist()
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);
            var sourceFilePath = "C:\\xy.json";

            var result = Program.Run(
                new[]
                {
                    sourceFilePath,
                    "C:\\abc.json"
                },
                services.BuildServiceProvider());

            Assert.Equal(1, result);
            TestHelper.VerifyLogMessage(logger, string.Format(LogMessages.SourceFileNotFound, sourceFilePath), LogLevel.Error);
            logger.VerifyNoOtherCalls();
        }

        [Fact]
        public void ResultFilePathCannotBeEmpty()
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);
            var tempFile = Path.GetTempFileName();

            var result = Program.Run(
                new[]
                {
                    tempFile,
                    "      "
                },
                services.BuildServiceProvider());

            Assert.Equal(1, result);
            TestHelper.VerifyLogMessage(logger, LogMessages.EmptyResultFilePath, LogLevel.Error);
            logger.VerifyNoOtherCalls();
        }

        [Fact]
        public void ResultFilePathCannotBeDirectory()
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);
            var tempFile = Path.GetTempFileName();
            var folder = "C:\\folder";

            var result = Program.Run(
                new[]
                {
                    tempFile,
                    folder
                },
                services.BuildServiceProvider());

            Assert.Equal(1, result);
            TestHelper.VerifyLogMessage(logger, string.Format(LogMessages.InvalidResultFilePath, folder), LogLevel.Error);
            logger.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        public void BatteryValidation(int battery, int expectedResult)
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);
            ConfigureFileService(services, battery);

            var actualResult = Program.Run(
                new[]
                {
                    Path.GetTempFileName(),
                    "C:\\abc.json"
                },
                services.BuildServiceProvider());

            Assert.Equal(expectedResult, actualResult);

            if (actualResult > 0)
            {
                TestHelper.VerifyLogMessage(logger, LogMessages.OutOfBattery, LogLevel.Error);
            }

            logger.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(2, 0, 1)]
        [InlineData(3, 0, 1)]
        public void PositionValidation(int x, int y, int expectedResult)
        {
            var logger = new Mock<ILogger<Program>>();
            var services = ConfigureServices(logger);
            ConfigureFileService(services, 100, x, y);

            var actualResult = Program.Run(
                new[]
                {
                    Path.GetTempFileName(),
                    "C:\\abc.json"
                },
                services.BuildServiceProvider());

            Assert.Equal(expectedResult, actualResult);

            if (actualResult > 0)
            {
                TestHelper.VerifyLogMessage(logger, string.Format(LogMessages.InvalidStartingPoint, new PositionWithDirection { X = x, Y = y }), LogLevel.Error);
            }

            logger.VerifyNoOtherCalls();
        }

        private static ServiceCollection ConfigureServices<T>(Mock<ILogger<T>> logger)
        {
            var services = Program.ConfigureServices();
            services.AddTransient(sp => logger.Object);
            return services;
        }

        private static void ConfigureFileService(ServiceCollection services, int battery, int x = 0, int y = 0)
        {
            services.RemoveAll<IFileService>();

            var fileService = new Mock<IFileService>();
            fileService.Setup(f => f.Read(It.IsAny<string>())).Returns(new CommandCleaningSettings
            {
                Battery = battery,
                Map = new List<List<MapCell?>>
                    {
                        new List<MapCell?> { MapCell.S }
                    },
                Start = new PositionWithDirection { X = x, Y = y },
                Commands = new List<Command> { Command.A }
            });

            services.AddTransient(sp => fileService.Object);
        }
    }
}