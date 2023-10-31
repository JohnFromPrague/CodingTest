namespace CleaningRobot.Models.Commands
{
    internal class RotationCommand : RobotCommand
    {
        public required bool Left { get; init; }

        public Direction GetDirection(Direction currentDirection)
        {
            var directionChange = Left ? -1 : 1;

            var newDirection = currentDirection + directionChange;
            if (!Enum.IsDefined(newDirection))
            {
                // Do not hardcode here W/N and use Max/Min instead.
                newDirection = directionChange < 0 ? Enum.GetValues<Direction>().Max() : Enum.GetValues<Direction>().Min();
            }

            return newDirection;
        }
    }
}
