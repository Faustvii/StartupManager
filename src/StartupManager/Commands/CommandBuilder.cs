using System.CommandLine;
using System.CommandLine.Invocation;
using StartupManager.Commands.List;

namespace StartupManager.Commands {
    public static class CommandBuilder {
        public static RootCommand GetRootCommand() => new RootCommand {
            GetStartupListCommand()
        };

        private static Command GetStartupListCommand() {
            var listCommand = new Command("list") {
                Description = "Lists the current startup programs"
            };

            listCommand.AddAlias("l");

            listCommand.Handler = CommandHandler.Create(ListCommandHandler.Run);

            return listCommand;
        }
    }
}