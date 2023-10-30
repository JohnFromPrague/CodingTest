using CleaningRobot.Json;
using CleaningRobot.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    static void Main(string[] args) => Run(args, ConfigureServices().BuildServiceProvider());

    internal static void Run(string[] args, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var fileService = serviceProvider.GetRequiredService<IFileService>();
            var filePaths = fileService.TryGetFilePaths(args);
            if (!filePaths.HasValue)
            {
                return;
            }

            var settings = fileService.Read(filePaths.Value.SourceFilePath);

            var robot = serviceProvider.GetRequiredService<IRobot>();
            if (!robot.Validate(settings))
            {
                return;
            }

            var result = robot.Run(settings);

            fileService.Write(filePaths.Value.ResultFilePath, result);
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