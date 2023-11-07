namespace CleaningRobot.Models
{
    internal class BacktrackingCleaningSettings : ICleaningSettings
    {
        public required List<List<MapCell?>> Map { get; set; }

        public required PositionWithDirection Start { get; set; }

        public required int Battery { get; set; }
    }
}
