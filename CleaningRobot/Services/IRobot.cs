using CleaningRobot.Model;

namespace CleaningRobot.Services
{
    internal interface IRobot
    {
        bool Validate(CleaningSettings settings);

        CleaningSession Run(CleaningSettings settings);
    }
}