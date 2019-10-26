namespace StartupManager.Commands.StartupToggle {
    public static class DisableCommand {
        public static void Run(string name) {
            EnableDisableCommandHandler.Run(name, false);
        }
    }
}