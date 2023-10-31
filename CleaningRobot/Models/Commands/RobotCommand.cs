namespace CleaningRobot.Models.Commands
{
    internal abstract class RobotCommand
    {
        public required int BatteryConsuption { get; init; }
    }
}
