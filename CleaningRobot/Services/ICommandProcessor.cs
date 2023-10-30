using CleaningRobot.Model;

namespace CleaningRobot.Services
{
    internal interface ICommandProcessor
    {
        bool IsObstacle(IReadOnlyList<IReadOnlyList<MapCell?>> map, int x, int y);

        CommandResult Process(IReadOnlyList<IReadOnlyList<MapCell?>> map, CleaningSession session, Command command);
    }
}