using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

using System.Diagnostics;

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
                var nextPosition = movementCommand.GetNextPosition(session.Position, session.Position.Facing);

                if (map.ContainsObstacle(nextPosition.X, nextPosition.Y))
                {
                    return CommandResult.Obstacle;
                }

                Trace.WriteLine(session.Position);

                session.Position.X = nextPosition.X;
                session.Position.Y = nextPosition.Y;
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
