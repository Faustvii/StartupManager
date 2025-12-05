using StartupManager.ConsoleOutputters;

namespace StartupManager.Commands.SetPriority {
    public static class SetPriorityCommand {
        public static void Run(string name, string priority) {
            var messages = SetPriorityCommandHandler.Run(name, priority);
            ConsoleOutputWriter.WriteToConsole(messages);
        }
    }
}

