using CleaningRobot.Model;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]

namespace CleaningRobot.Services
{
    internal class FileService : IFileService
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<FileService> logger;

        public FileService(JsonSerializerOptions jsonSerializerOptions, ILogger<FileService> logger)
        {
            this.jsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } 

        public (string SourceFilePath, string ResultFilePath)? TryGetFilePaths(string[] files)
        {
            if (files == null || files.Length != 2)
            {
                this.logger.LogError(LogMessages.TwoParametersRequired);
                return null;
            }

            var sourceFilePath = files[0];
            if (!File.Exists(sourceFilePath))
            {
                this.logger.LogError(string.Format(LogMessages.SourceFileNotFound, sourceFilePath));
                return null;
            }

            var resultFilePath = files[1];
            if (string.IsNullOrWhiteSpace(resultFilePath))
            {
                this.logger.LogError(LogMessages.EmptyResultFilePath);
                return null;
            }

            if (!Path.HasExtension(resultFilePath))
            {
                this.logger.LogError(string.Format(LogMessages.InvalidResultFilePath, resultFilePath));
                return null;
            }

            return (sourceFilePath, resultFilePath);
        }

        public CleaningSettings Read(string sourceFilePath)
        {
            using var sourceFileStream = File.OpenRead(sourceFilePath);
            var cleaningArgs = JsonSerializer.Deserialize<CleaningSettings>(sourceFileStream, this.jsonSerializerOptions);
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
