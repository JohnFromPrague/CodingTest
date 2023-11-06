using CleaningRobot.Models;

namespace CleaningRobot.Services.Strategy
{
    internal interface ICleaningSettings
    {
        PositionWithDirection Start { get; }

        int Battery { get; }
    }
}