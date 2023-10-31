namespace CleaningRobot.Models.Commands
{
    internal abstract class CommandBase
    {
        public required int BatteryConsuption { get; init; }
    }
}
