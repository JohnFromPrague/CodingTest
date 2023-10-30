using CleaningRobot.Model;
using CleaningRobot.Models;

using System.Drawing;

namespace CleaningRobot.Services
{
    internal class CommandProcessor : ICommandProcessor
    {
        public CommandResult Process(IReadOnlyList<IReadOnlyList<MapCell?>> map, CleaningSession session, Command command)
        {
            var batteryConsuption = GetBatteryConsuption(command);
            if (session.Battery - batteryConsuption < 0)
            {
                return CommandResult.LowBattery;
            }

            session.Battery -= batteryConsuption;

            if (command == Command.TL || command == Command.TR)
            {
                session.Position.Direction = GetNewDirection(session.Position.Direction, command);
            }

            if (command == Command.A || command == Command.B)
            {
                var nextStep = GetNextStep(session.Position.Direction, command == Command.A);
                var nextX = session.Position.X + nextStep.X;
                var nextY = session.Position.Y + nextStep.Y;

                if (IsObstacle(map, nextX, nextY))
                {
                    return CommandResult.Obstacle;
                }

                session.Position.X = nextX;
                session.Position.Y = nextY;
            }

            if (command == Command.C)
            {
                session.Cleaned.Add(new Position { X = session.Position.X, Y = session.Position.Y });
            }

            session.Visited.Add(new Position { X = session.Position.X, Y = session.Position.Y });

            return CommandResult.Success;
        }

        public bool IsObstacle(IReadOnlyList<IReadOnlyList<MapCell?>> map, int x, int y)
        {
            return x < 0 || y < 0 || y >= map.Count || x >= map[y].Count || map[y][x] != MapCell.S;
        }

        internal static Direction GetNewDirection(Direction currentDirection, Command command)
        {
            var directionChange = command switch
            {
                Command.TL => -1,
                Command.TR => 1,
                _ => throw new InvalidOperationException($"Command {command} is not supported")
            };

            var newDirection = currentDirection + directionChange;
            if (!Enum.IsDefined(newDirection))
            {
                // Do not hardcode here W/N and use Max/Min instead.
                newDirection = directionChange < 0 ? Enum.GetValues<Direction>().Max() : Enum.GetValues<Direction>().Min();
            }

            return newDirection;
        }

        internal static Point GetNextStep(Direction direction, bool advance)
        {
            return direction switch
            {
                Direction.N => new Point(0, advance ? -1 : 1),
                Direction.E => new Point(advance ? 1 : -1, 0),
                Direction.S => new Point(0, advance ? 1 : -1),
                Direction.W => new Point(advance ? -1 : 1, 0),
                _ => throw new InvalidOperationException($"Direction {direction} is not supported")
            };
        }

        private static int GetBatteryConsuption(Command command)
        {
            return command switch
            {
                Command.TL => 1,
                Command.TR => 1,
                Command.A => 2,
                Command.B => 3,
                Command.C => 5,
                _ => throw new InvalidOperationException($"Command {command} is not supported")
            };
        }

    }

    public enum CommandResult
    {
        Success,
        LowBattery,
        Obstacle
    }
}
