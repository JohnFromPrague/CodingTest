namespace CleaningRobot.Models.Commands
{
    internal static class CommandExtensions
    {
        // No need to create multiple instances of commands.
        private static RobotCommand turnLeft = new RotationCommand { BatteryConsuption = 1, Left = true };
        private static RobotCommand turnRight = new RotationCommand { BatteryConsuption = 1, Left = false };
        private static RobotCommand advance = new MovementCommand { BatteryConsuption = 2, Advance = true };
        private static RobotCommand back = new MovementCommand { BatteryConsuption = 3, Advance = false };
        private static RobotCommand clean = new CleanCommand { BatteryConsuption = 5 };

        internal static RobotCommand AsRobotCommand(this Command command)
        {
            return command switch
            {
                Command.TL => turnLeft,
                Command.TR => turnRight,
                Command.A => advance,
                Command.B => back,
                Command.C => clean,
                _ => throw new InvalidOperationException($"Command {command} is not supported")
            };
        }
    }
}
