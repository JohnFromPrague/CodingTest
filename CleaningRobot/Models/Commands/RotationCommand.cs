namespace CleaningRobot.Models.Commands
{
    internal class RotationCommand : CommandBase
    {
        /// <summary>
        /// Change of direction within <see cref="Direction"/> enum, where -1 means turn to left and +1 means turn to right.
        /// </summary>
        public required int DirectionChange { get; init; }

        public Direction GetDirection(Direction currentDirection)
        {
            var newDirection = currentDirection + DirectionChange;

            if (!Enum.IsDefined(newDirection))
            {
                var min = Enum.GetValues<Direction>().Min();
                var max = Enum.GetValues<Direction>().Max();

                do
                {
                    newDirection = DirectionChange < 0 ?
                        (int)max + newDirection + 1 :
                        (int)newDirection - max - 1;
                }
                while (!Enum.IsDefined(newDirection));
            }

            return newDirection;
        }
    }
}
