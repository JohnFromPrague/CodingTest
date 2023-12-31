﻿using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

using Microsoft.Extensions.Logging;

namespace CleaningRobot.Services
{
    internal class Robot : IRobot
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly ILogger<Robot> logger;
        private readonly IEnumerable<IEnumerable<Command>> backOffStrategy = new[]
        {
            new[] { Command.TR, Command.A, Command.TL },
            new[] { Command.TR, Command.A, Command.TR },
            new[] { Command.TR, Command.A, Command.TR },
            new[] { Command.TR, Command.B, Command.TR, Command.A },
            new[] { Command.TL, Command.TL, Command.A }
        };

        public Robot(ICommandProcessor commandProcessor, ILogger<Robot> logger)
        {
            this.commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Runs robot with provided cleaning settings.
        /// </summary>
        public CleaningSession Run(CleaningSettings settings)
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
