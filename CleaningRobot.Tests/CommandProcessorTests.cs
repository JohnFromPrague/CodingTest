using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

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
            var actualDirection = ((RotationCommand)command.AsRobotCommand()).GetDirection(direction);
            Assert.Equal(expectedDirection, actualDirection);
        }

        [Theory]
        [InlineData(Direction.N, -4, Direction.N)]
        [InlineData(Direction.N, -5, Direction.W)]
        [InlineData(Direction.N, -6, Direction.S)]
        [InlineData(Direction.N, -7, Direction.E)]
        [InlineData(Direction.N, 4, Direction.N)]
        [InlineData(Direction.N, 5, Direction.E)]
        [InlineData(Direction.N, 6, Direction.S)]
        [InlineData(Direction.N, 7, Direction.W)]
        public void ComplexDirectionTest(Direction direction, int directionChange, Direction expectedDirection)
        {
            var command = new RotationCommand { BatteryConsuption = 1, DirectionChange = directionChange };
            Assert.Equal(expectedDirection, command.GetDirection(direction));
        }

        [Theory]
        [InlineData(Direction.N, Command.A, 0, -1)]
        [InlineData(Direction.E, Command.A, 1, 0)]
        [InlineData(Direction.S, Command.A, 0, 1)]
        [InlineData(Direction.W, Command.A, -1, 0)]
        [InlineData(Direction.N, Command.B, 0, 1)]
        [InlineData(Direction.E, Command.B, -1, 0)]
        [InlineData(Direction.S, Command.B, 0, -1)]
        [InlineData(Direction.W, Command.B, 1, 0)]
        public void StepTest(Direction direction, Command command, int expectedX, int expectedY)
        {
            var nextStep = ((MovementCommand)command.AsRobotCommand()).GetStep(direction);
            Assert.Equal(expectedX, nextStep.X);
            Assert.Equal(expectedY, nextStep.Y);
        }

        [Theory]
        [InlineData(Direction.N, 5, 0, -5)]
        [InlineData(Direction.E, 5, 5, 0)]
        [InlineData(Direction.S, 5, 0, 5)]
        [InlineData(Direction.W, 5, -5, 0)]
        [InlineData(Direction.N, -5, 0, 5)]
        [InlineData(Direction.E, -5, -5, 0)]
        [InlineData(Direction.S, -5, 0, -5)]
        [InlineData(Direction.W, -5, 5, 0)]
        public void ComplexStepTest(Direction direction, int distanceChange, int expectedX, int expectedY)
        {
            var command = new MovementCommand { BatteryConsuption = 1, DistanceChange = distanceChange };
            var nextStep = command.GetStep(direction);

            Assert.Equal(expectedX, nextStep.X);
            Assert.Equal(expectedY, nextStep.Y);
        }
    }
}
