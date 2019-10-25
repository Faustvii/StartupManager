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

            var detailedOption = new Option("--detailed", "Shows additional output about the startup programs");
            detailedOption.AddAlias("-d");
            listCommand.AddOption(detailedOption);

            listCommand.Handler = CommandHandler.Create<bool>(ListCommandHandler.Run);

            return listCommand;
        }
    }
}