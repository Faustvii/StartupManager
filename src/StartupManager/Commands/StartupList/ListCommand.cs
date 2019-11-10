using System.Linq;
using StartupManager.ConsoleOutputters;

namespace StartupManager.Commands.StartupList {
    public static class ListCommand {
        public static void Run(bool detailed) {
            var result = ListCommandHandler.Run();
            if (result.Messages.Any()) {
                ConsoleOutputWriter.WriteToConsole(result.Messages);
            }
            ConsoleOutputWriter.WriteToConsole(detailed, result.Programs);
        }
    }
}