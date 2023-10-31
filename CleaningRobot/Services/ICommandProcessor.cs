using CleaningRobot.Models;
using CleaningRobot.Models.Commands;

namespace CleaningRobot.Services
{
    internal interface ICommandProcessor
    {
        CommandResult Process(IReadOnlyList<IReadOnlyList<MapCell?>> map, CleaningSession session, RobotCommand command);
    }
}