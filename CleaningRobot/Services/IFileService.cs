using CleaningRobot.Models;

namespace CleaningRobot.Services
{
    internal interface IFileService
    {
        CleaningSettings Read(string sourceFilePath);

        void Write(string sourceFilePath, CleaningSession cleaningResult);
    }
}