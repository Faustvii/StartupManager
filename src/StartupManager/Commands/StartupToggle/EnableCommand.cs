using StartupManager.ConsoleOutputters;

namespace StartupManager.Commands.StartupToggle {
    public static class EnableCommand {
        public static void Run(string name) {
            var messages = EnableDisableCommandHandler.Run(name, true);
            ConsoleOutputWriter.WriteToConsole(messages);
        }
    }
}