using CleaningRobot.Models;

namespace CleaningRobot.Services.Strategy
{
    internal interface ICleaningStrategy<TCleaningSettings>
        where TCleaningSettings : ICleaningSettings
    {
        CleaningSession Run(TCleaningSettings settings);
    }
}
