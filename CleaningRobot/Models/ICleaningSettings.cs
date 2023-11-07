namespace CleaningRobot.Models
{
    internal interface ICleaningSettings
    {
        PositionWithDirection Start { get; }

        int Battery { get; }
    }
}