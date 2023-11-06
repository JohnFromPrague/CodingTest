using CleaningRobot.Models;
using CleaningRobot.Models.Commands;
using CleaningRobot.Services;
using CleaningRobot.Services.Strategy;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleaningRobot.Tests
{
    public class CommandCleaningStrategyTests
    {
        [Theory]
        [InlineData(5)]
        [InlineData(10000)]
        public void RobotStuck(int battery)
        {
            var logger = new Mock<ILogger<CommandCleaningStrategy>>();

            var commandProcessor = new CommandCleaningStrategy(new CommandProcessor(), logger.Object);
            commandProcessor.Run(
                new CleaningSettings
                {
                    Battery = battery,
                    Map = new List<List<MapCell?>>
                    {
                        new List<MapCell?> { MapCell.S }
                    },
                    Start = new PositionWithDirection { X = 0, Y = 0 },
                    Commands = new List<Command> { Command.A }
                });

            TestHelper.VerifyLogMessage(logger, LogMessages.RobotStuck, LogLevel.Error);
        }
    }
}
