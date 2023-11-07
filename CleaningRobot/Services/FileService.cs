using CleaningRobot.Models;

using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace CleaningRobot.Services
{
    internal class FileService : IFileService
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public FileService(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        } 

        public CommandCleaningSettings Read(string sourceFilePath)
        {
            using var sourceFileStream = File.OpenRead(sourceFilePath);
            var cleaningArgs = JsonSerializer.Deserialize<CommandCleaningSettings>(sourceFileStream, this.jsonSerializerOptions);
            if (cleaningArgs == null)
            {
                throw new InvalidOperationException("Failed to deserialize source file.");
            }

            return cleaningArgs;
        }

        public void Write(string resultFilePath, CleaningSession cleaningResult)
        {
            using var resultFileStream = File.OpenWrite(resultFilePath);
            JsonSerializer.Serialize(resultFileStream, cleaningResult, this.jsonSerializerOptions);
        }
    }
}
