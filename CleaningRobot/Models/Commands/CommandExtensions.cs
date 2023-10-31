namespace CleaningRobot.Models.Commands
{
    internal static class CommandExtensions
    {
        // No need to create multiple instances of commands.
        private static CommandBase turnLeft = new RotationCommand { BatteryConsuption = 1, DirectionChange = -1 };
        private static CommandBase turnRight = new RotationCommand { BatteryConsuption = 1, DirectionChange = 1 };
        private static CommandBase advance = new MovementCommand { BatteryConsuption = 2, DistanceChange = 1 };
        private static CommandBase back = new MovementCommand { BatteryConsuption = 3, DistanceChange = -1 };
        private static CommandBase clean = new CleanCommand { BatteryConsuption = 5 };

        internal static CommandBase AsRobotCommand(this Command command)
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
