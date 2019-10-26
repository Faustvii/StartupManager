using StartupManager.ConsoleOutputters;

namespace StartupManager.Commands.StartupList {
    public static class ListCommand {
        public static void Run(bool detailed) {
            var startupPrograms = ListCommandHandler.Run(detailed);
            ConsoleOutputWriter.WriteToConsole(detailed, startupPrograms);
        }
    }
}