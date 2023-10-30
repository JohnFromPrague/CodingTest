using Castle.Core.Logging;

using CleaningRobot.Model;
using CleaningRobot.Services;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleaningRobot.Tests
{
    public class CommandProcessorTests
    {
        [Theory]
        [InlineData(Direction.N, Command.TL, Direction.W)]
        [InlineData(Direction.N, Command.TR, Direction.E)]
        [InlineData(Direction.E, Command.TL, Direction.N)]
        [InlineData(Direction.E, Command.TR, Direction.S)]
        [InlineData(Direction.S, Command.TL, Direction.E)]
        [InlineData(Direction.S, Command.TR, Direction.W)]
        [InlineData(Direction.W, Command.TL, Direction.S)]
        [InlineData(Direction.W, Command.TR, Direction.N)]
        public void DirectionTest(Direction direction, Command command, Direction expectedDirection)
        {
            var actualDirection = CommandProcessor.GetNewDirection(direction, command);
            Assert.Equal(expectedDirection, actualDirection);
        }

        [Theory]
        [InlineData(Direction.N, true, 0, -1)]
        [InlineData(Direction.E, true, 1, 0)]
        [InlineData(Direction.S, true, 0, 1)]
        [InlineData(Direction.W, true, -1, 0)]
        [InlineData(Direction.N, false, 0, 1)]
        [InlineData(Direction.E, false, -1, 0)]
        [InlineData(Direction.S, false, 0, -1)]
        [InlineData(Direction.W, false, 1, 0)]
        public void StepTest(Direction direction, bool advance, int expectedX, int expectedY)
        {
            var nextStep = CommandProcessor.GetNextStep(direction, advance);
            Assert.Equal(expectedX, nextStep.X);
            Assert.Equal(expectedY, nextStep.Y);
        }
    }
}
