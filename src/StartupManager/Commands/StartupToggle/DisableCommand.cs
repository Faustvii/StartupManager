using StartupManager.ConsoleOutputters;

namespace StartupManager.Commands.StartupToggle {
    public static class DisableCommand {
        public static void Run(string name) {
            var messages = EnableDisableCommandHandler.Run(name, false);
            ConsoleOutputWriter.WriteToConsole(messages);
        }
    }
}