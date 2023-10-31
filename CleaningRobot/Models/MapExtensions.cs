namespace CleaningRobot.Models
{
    public static class MapExtensions
    {
        public static bool ContainsObstacle(this IReadOnlyList<IReadOnlyList<MapCell?>> map, int x, int y)
        {
            return x < 0 || y < 0 || y >= map.Count || x >= map[y].Count || map[y][x] != MapCell.S;
        }
    }
}
