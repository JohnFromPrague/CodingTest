using CleaningRobot.Models;

namespace CleaningRobot.Services
{
    internal interface IFileService
    {
        CommandCleaningSettings Read(string sourceFilePath);

        void Write(string sourceFilePath, CleaningSession cleaningResult);
    }
}