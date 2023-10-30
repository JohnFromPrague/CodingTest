using CleaningRobot.Model;
using CleaningRobot.Services;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleaningRobot.Tests
{
    public class RobotTests
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void BatteryValidation(int battery, bool expectedValidationResult)
        {
            var logger = new Mock<ILogger<Robot>>();

            var commandProcessor = new Robot(new CommandProcessor(), logger.Object);
            var actualValidationResult = commandProcessor.Validate(
                new CleaningSettings
                {
                    Battery = battery,
                    Map = new List<List<MapCell?>>
                    {
                        new List<MapCell?> { MapCell.S }
                    },
                    Position = new PositionWithDirection { X = 0, Y = 0 },
                    Commands = new List<Command> { Command.A }
                });

            Assert.Equal(expectedValidationResult, actualValidationResult);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(1, 0, false)]
        [InlineData(2, 0, false)]
        [InlineData(3, 0, false)]
        public void PositionValidation(int x, int y, bool expectedValidationResult)
        {
            var logger = new Mock<ILogger<Robot>>();

            var commandProcessor = new Robot(new CommandProcessor(), logger.Object);
            var actualValidationResult = commandProcessor.Validate(
                new CleaningSettings
                {
                    Battery = 1,
                    Map = new List<List<MapCell?>>
                    {
                        new List<MapCell?> { MapCell.S, MapCell.C, null }
                    },
                    Position = new PositionWithDirection { X = x, Y = y },
                    Commands = new List<Command> { Command.A }
                });

            Assert.Equal(expectedValidationResult, actualValidationResult);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10000)]
        public void RobotStuck(int battery)
        {
            var logger = new Mock<ILogger<Robot>>();

            var commandProcessor = new Robot(new CommandProcessor(), logger.Object);
            commandProcessor.Run(
                new CleaningSettings
                {
                    Battery = battery,
                    Map = new List<List<MapCell?>>
                    {
                        new List<MapCell?> { MapCell.S }
                    },
                    Position = new PositionWithDirection { X = 0, Y = 0 },
                    Commands = new List<Command> { Command.A }
                });

            TestHelper.VerifyLogMessage(logger, LogMessages.RobotStuck, LogLevel.Error);
        }
    }
}
