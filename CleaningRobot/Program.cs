using CleaningRobot;
using CleaningRobot.Json;
using CleaningRobot.Models;
using CleaningRobot.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    static int Main(string[] args) => Run(args, ConfigureServices().BuildServiceProvider());

    internal static int Run(string[] args, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var fileService = serviceProvider.GetRequiredService<IFileService>();
            var robot = serviceProvider.GetRequiredService<IRobot>();

            if (args == null || args.Length != 2)
            {
                logger.LogError(LogMessages.TwoParametersRequired);
                return 1;
            }

            var sourceFilePath = args[0];
            if (!File.Exists(sourceFilePath))
            {
                logger.LogError(string.Format(LogMessages.SourceFileNotFound, sourceFilePath));
                return 1;
            }

            var resultFilePath = args[1];
            if (string.IsNullOrWhiteSpace(resultFilePath))
            {
                logger.LogError(LogMessages.EmptyResultFilePath);
                return 1;
            }

            if (!Path.HasExtension(resultFilePath))
            {
                logger.LogError(string.Format(LogMessages.InvalidResultFilePath, resultFilePath));
                return 1;
            }

            var settings = fileService.Read(sourceFilePath);
            if (settings.Battery <= 0)
            {
                logger.LogError(LogMessages.OutOfBattery);
                return 1;
            }

            if (settings.Map.ContainsObstacle(settings.Start.X, settings.Start.Y))
            {
                logger.LogError(string.Format(LogMessages.InvalidStartingPoint, settings.Start));
                return 1;
            }

            var result = robot.Run(settings);
            fileService.Write(resultFilePath, result);

            return 0;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled exception");
            throw;
        }
    }

    internal static ServiceCollection ConfigureServices()
    {
        var collection = new ServiceCollection();
        collection.AddTransient<IFileService, FileService>();
        collection.AddTransient<IRobot, Robot>();
        collection.AddTransient<ICommandProcessor, CommandProcessor>();

        collection.AddSingleton(new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(), new IgnoreEmptyStringNullableEnumConverterFactory() }

        });

        collection.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Trace));
        return collection;
    }
}