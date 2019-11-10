using StartupManager.ConsoleOutputters;

namespace StartupManager.Commands.Remove {
    public static class RemoveCommand {
        public static void Run(string name, bool confirm) {
            var result = RemoveCommandHandler.Run(name, confirm);
            ConsoleOutputWriter.WriteToConsole(result);
        }
    }
}