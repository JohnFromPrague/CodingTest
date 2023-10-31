namespace CleaningRobot.Models.Commands
{
    internal class MovementCommand : RobotCommand
    {
        public required bool Advance { get; init; }

        public Position GetStep(Direction direction)
        {
            return direction switch
            {
                Direction.N => new Position { X = 0, Y = Advance ? -1 : 1 },
                Direction.E => new Position { X = Advance ? 1 : -1, Y = 0 },
                Direction.S => new Position { X = 0, Y = Advance ? 1 : -1 },
                Direction.W => new Position { X = Advance ? -1 : 1, Y = 0 },
                _ => throw new InvalidOperationException($"Direction {direction} is not supported")
            };
        }
    }
}
