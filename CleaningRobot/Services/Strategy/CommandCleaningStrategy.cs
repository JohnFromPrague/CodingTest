using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

using Microsoft.Extensions.Logging;

namespace CleaningRobot.Services.Strategy
{
    internal class CommandCleaningStrategy : ICleaningStrategy<CommandCleaningSettings>
    {
        private readonly IEnumerable<IEnumerable<Command>> backOffStrategy = new[]
        {
            new[] { Command.TR, Command.A, Command.TL },
            new[] { Command.TR, Command.A, Command.TR },
            new[] { Command.TR, Command.A, Command.TR },
            new[] { Command.TR, Command.B, Command.TR, Command.A },
            new[] { Command.TL, Command.TL, Command.A }
        };

        private readonly ILogger<CommandCleaningStrategy> logger;
        private readonly ICommandProcessor commandProcessor;

        public CommandCleaningStrategy(ICommandProcessor commandProcessor, ILogger<CommandCleaningStrategy> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
        }

        public CleaningSession Run(CommandCleaningSettings settings)
        {
            var cleaningSession = new CleaningSession { Position = settings.Start, Battery = settings.Battery };
            this.logger.LogInformation($"Starting position {cleaningSession.Position} with {cleaningSession.Battery} battery");

            var commands = new Queue<Command>(settings.Commands);

            while (commands.TryDequeue(out var command))
            {
                this.logger.LogInformation($"Processing command: {command}");
                var result = this.commandProcessor.Process(settings.Map, cleaningSession, command.AsRobotCommand());
                this.logger.LogInformation($"Result: {result}, position: {cleaningSession.Position}, battery: {cleaningSession.Battery}");

                if (result == CommandResult.Obstacle)
                {
                    foreach (var backOffCommands in this.backOffStrategy)
                    {
                        foreach (var backoffCommand in backOffCommands)
                        {
                            this.logger.LogWarning($"Processing back off strategy command: {backoffCommand}");
                            result = this.commandProcessor.Process(settings.Map, cleaningSession, backoffCommand.AsRobotCommand());
                            this.logger.LogWarning($"Result: {result}, {cleaningSession.Position}, battery: {cleaningSession.Battery}");

                            if (result != CommandResult.Success)
                            {
                                break;
                            }
                        }

                        if (result != CommandResult.Obstacle)
                        {
                            break;
                        }
                    }

                    if (result != CommandResult.Success)
                    {
                        this.logger.LogError(LogMessages.RobotStuck);

                        break;
                    }
                }
            }

            return cleaningSession;
        }
    }
}
