using CleaningRobot.Model;

namespace CleaningRobot.Services
{
    internal interface IFileService
    {
        (string SourceFilePath, string ResultFilePath)? TryGetFilePaths(string[] files);

        CleaningSettings Read(string sourceFilePath);

        void Write(string sourceFilePath, CleaningSession cleaningResult);
    }
}