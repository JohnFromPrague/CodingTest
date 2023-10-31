using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

namespace CleaningRobot.Services
{
    internal class CommandProcessor : ICommandProcessor
    {
        public CommandResult Process(IReadOnlyList<IReadOnlyList<MapCell?>> map, CleaningSession session, CommandBase command)
        {
            if (session.Battery - command.BatteryConsuption < 0)
            {
                return CommandResult.LowBattery;
            }

            session.Battery -= command.BatteryConsuption;

            if (command is RotationCommand rotationCommand)
            {
                session.Position.Facing = rotationCommand.GetDirection(session.Position.Facing);
            }
            else if (command is MovementCommand movementCommand)
            {
                var path = movementCommand.GetStep(session.Position.Facing);
                var nextX = session.Position.X + path.X;
                var nextY = session.Position.Y + path.Y;

                if (map.ContainsObstacle(nextX, nextY))
                {
                    return CommandResult.Obstacle;
                }

                session.Position.X = nextX;
                session.Position.Y = nextY;
            }
            else if (command is CleanCommand)
            {
                session.Cleaned.Add(new Position { X = session.Position.X, Y = session.Position.Y });
            }

            session.Visited.Add(new Position { X = session.Position.X, Y = session.Position.Y });

            return CommandResult.Success;
        }
    }
}
