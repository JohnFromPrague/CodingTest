using CleaningRobot.Models;

namespace CleaningRobot.Services
{
    internal interface IRobot
    {
        CleaningSession Run(CleaningSettings settings);
    }
}