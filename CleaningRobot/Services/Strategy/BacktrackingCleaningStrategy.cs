using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

namespace CleaningRobot.Services.Strategy
{
    internal class BacktrackingCleaningStrategy : ICleaningStrategy<BacktrackingCleaningSettings>
    {
        private readonly ICommandProcessor commandProcessor;

        public BacktrackingCleaningStrategy(ICommandProcessor commandProcessor)
        {
            this.commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
        }

        public CleaningSession Run(BacktrackingCleaningSettings settings)
        {
            var session = new CleaningSession { Battery = settings.Battery, Position = settings.Start };

            var cleanable = new HashSet<Position>(settings.Map.SelectMany((row, y) => row.Select((cell, x) => new { x, y, cell })).Where(x => x.cell == MapCell.S).Select(x => new Position { X = x.x, Y = x.y }));
            var reverseCommands = new Stack<Command>();

            while (!session.Cleaned.SequenceEqual(cleanable))
            {
                // We should clean only position which is not cleaned yet
                if (!session.Cleaned.Contains(session.Position))
                {
                    if (this.commandProcessor.Process(settings.Map, session, Command.C.AsRobotCommand()) == CommandResult.LowBattery)
                    {
                        return session;
                    }
                }

                var advanceCommand = (MovementCommand)Command.A.AsRobotCommand();

                // We should advance only to position which is unvisited
                if (IsNextPositionUnvisited(session, advanceCommand))
                {
                    var result = this.commandProcessor.Process(settings.Map, session, advanceCommand);
                    if (result == CommandResult.Success)
                    {
                        reverseCommands.Push(Command.B);
                        continue;
                    }
                    else if (result == CommandResult.Obstacle)
                    {
                        // In case of obstacle we should try all directions around us
                        result = ProcessObstacle(settings, session, reverseCommands, advanceCommand);
                    }

                    if (result == CommandResult.LowBattery)
                    {
                        return session;
                    }
                }
                else
                {
                    // In case there is no available position around us we should go back
                    Command reverseCommand;

                    do
                    {
                        if (!reverseCommands.TryPop(out reverseCommand) || this.commandProcessor.Process(settings.Map, session, reverseCommand.AsRobotCommand()) == CommandResult.LowBattery)
                        {
                            return session;
                        }
                    }
                    while (reverseCommand != Command.B);

                    if (this.commandProcessor.Process(settings.Map, session, Command.TL.AsRobotCommand()) == CommandResult.LowBattery)
                    {
                        return session;
                    }

                    reverseCommands.Push(Command.TR);
                }
            }

            return session;
        }

        private static bool IsNextPositionUnvisited(CleaningSession session, MovementCommand advanceCommand)
            => !session.Visited.Contains(advanceCommand.GetNextPosition(session.Position, session.Position.Facing));

        private CommandResult ProcessObstacle(BacktrackingCleaningSettings settings, CleaningSession session, Stack<Command> reverseCommands, MovementCommand advanceCommand)
        {
            CommandResult result = CommandResult.Success;

            for (var i = 0; i < 3; i++)
            {
                result = this.commandProcessor.Process(settings.Map, session, Command.TL.AsRobotCommand());
                if (result == CommandResult.LowBattery)
                {
                    break;
                }

                reverseCommands.Push(Command.TR);

                if (IsNextPositionUnvisited(session, advanceCommand))
                {
                    result = this.commandProcessor.Process(settings.Map, session, advanceCommand);
                    if (result == CommandResult.Obstacle)
                    {
                        continue;
                    }

                    if (result == CommandResult.Success)
                    {
                        reverseCommands.Push(Command.B);
                    }

                    break;
                }
            }

            return result;
        }
    }
}
