namespace CleaningRobot.Models.Commands
{
    internal class MovementCommand : CommandBase
    {
        /// <summary>
        /// Change of distance in specific direction. Positive number means advance, negative number means back.
        /// </summary>
        public required int DistanceChange { get; init; }

        public Position GetStep(Direction direction)
        {
            return direction switch
            {
                Direction.N => new Position { X = 0, Y = -DistanceChange },
                Direction.E => new Position { X = DistanceChange, Y = 0 },
                Direction.S => new Position { X = 0, Y = DistanceChange },
                Direction.W => new Position { X = -DistanceChange, Y = 0 },
                _ => throw new InvalidOperationException($"Direction {direction} is not supported")
            };
        }

        public Position GetNextPosition(Position current, Direction direction)
        {
            var path = GetStep(direction);
            return new Position { X = current.X + path.X, Y = current.Y + path.Y };
        }
    }
}
