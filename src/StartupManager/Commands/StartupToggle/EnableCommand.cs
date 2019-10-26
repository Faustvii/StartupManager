namespace StartupManager.Commands.StartupToggle {
    public static class EnableCommand {
        public static void Run(string name) {
            EnableDisableCommandHandler.Run(name, true);
        }
    }
}