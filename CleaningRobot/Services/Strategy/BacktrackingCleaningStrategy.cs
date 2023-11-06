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
                if (!session.Cleaned.Contains(session.Position))
                {
                    if (this.commandProcessor.Process(settings.Map, session, Command.C.AsRobotCommand()) == CommandResult.LowBattery)
                    {
                        return session;
                    }
                }

                if (Command.A.AsRobotCommand() is MovementCommand advanceCommand)
                {
                    if (!session.Visited.Contains(advanceCommand.GetNextPosition(session.Position, session.Position.Facing)))
                    {
                        var result = this.commandProcessor.Process(settings.Map, session, advanceCommand);
                        if (result == CommandResult.LowBattery)
                        {
                            return session;
                        }
                        else if (result == CommandResult.Obstacle)
                        {
                            for (var i = 0; i < 3; i++)
                            {
                                if (this.commandProcessor.Process(settings.Map, session, Command.TL.AsRobotCommand()) == CommandResult.LowBattery)
                                {
                                    return session;
                                }

                                reverseCommands.Push(Command.TR);

                                if (!session.Visited.Contains(advanceCommand.GetNextPosition(session.Position, session.Position.Facing)))
                                {
                                    result = this.commandProcessor.Process(settings.Map, session, advanceCommand);

                                    if (result == CommandResult.LowBattery)
                                    {
                                        return session;
                                    }
                                    else if (result == CommandResult.Success)
                                    {
                                        reverseCommands.Push(Command.B);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            reverseCommands.Push(Command.B);
                        }
                    }
                    else
                    {
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
            }

            return session;
        }
    }

    internal class BacktrackingCleaningSettings : ICleaningSettings
    {
        public required List<List<MapCell?>> Map { get; set; }

        public required PositionWithDirection Start { get; set; }

        public required int Battery { get; set; }
    }
}
