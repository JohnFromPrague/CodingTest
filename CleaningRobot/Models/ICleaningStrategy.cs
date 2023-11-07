namespace CleaningRobot.Models
{
    internal interface ICleaningStrategy<TCleaningSettings>
        where TCleaningSettings : ICleaningSettings
    {
        CleaningSession Run(TCleaningSettings settings);
    }
}
