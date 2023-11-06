using CleaningRobot.Models;
using CleaningRobot.Services;
using CleaningRobot.Services.Strategy;

namespace CleaningRobot.Tests
{
    public class AutonomousCleaningStrategyTests
    {
        /// <summary>
        /// X X * * *
        /// * * * * *
        /// X X * * *
        /// X * * X *
        /// * * * * X
        /// </summary>
        [Fact]
        public void Run()
        {
            var map = new List<List<MapCell?>>
            {
                new List<MapCell?> { MapCell.C, MapCell.C, MapCell.S, MapCell.S, MapCell.S },
                new List<MapCell?> { MapCell.S, MapCell.S, MapCell.S, MapCell.S, MapCell.S },
                new List<MapCell?> { MapCell.C, MapCell.C, MapCell.S, MapCell.S, MapCell.S },
                new List<MapCell?> { MapCell.C, MapCell.S, MapCell.S, MapCell.C, MapCell.S },
                new List<MapCell?> { MapCell.S, MapCell.S, MapCell.S, MapCell.S, MapCell.C },
            };

            var commandProcessor = new BacktrackingCleaningStrategy(new CommandProcessor());
            var session = commandProcessor.Run(
                new BacktrackingCleaningSettings
                {
                    Battery = 1000,
                    Map = map,
                    Start = new PositionWithDirection { X = 0, Y = 4, Facing = Direction.W },
                });

            var expectedCleanable = new HashSet<Position>(map.SelectMany((row, y) => row.Select((cell, x) => new { x, y, cell })).Where(x => x.cell == MapCell.S).Select(x => new Position { X = x.x, Y = x.y }).OrderBy(p => p.X).ThenBy(p => p.Y));
            Assert.Equal(expectedCleanable, session.Cleaned);
        }
    }
}
