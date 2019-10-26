using System.CommandLine;
using System.CommandLine.Invocation;
using StartupManager.Commands.StartupList;
using StartupManager.Commands.StartupToggle;

namespace StartupManager.Commands {
    public static class CommandBuilder {
        public static RootCommand GetRootCommand() => new RootCommand {
            GetStartupListCommand(),
            GetDisableStartupCommand(),
            GetEnableStartupCommand()
        };

        private static Command GetStartupListCommand() {
            var listCommand = new Command("list") {
                Description = "Lists the current startup programs"
            };

            listCommand.AddAlias("l");

            var detailedOption = new Option("--detailed", "Shows additional output about the startup programs");
            detailedOption.AddAlias("-d");
            listCommand.AddOption(detailedOption);

            listCommand.Handler = CommandHandler.Create<bool>(ListCommand.Run);

            return listCommand;
        }

        private static Command GetDisableStartupCommand() {
            var disableCommand = new Command("disable") {
                Description = "Disables one of the current startup programs"
            };

            disableCommand.AddAlias("d");

            disableCommand.AddArgument(new Argument<string>("name"));

            disableCommand.Handler = CommandHandler.Create<string>(DisableCommand.Run);

            return disableCommand;
        }

        private static Command GetEnableStartupCommand() {
            var disableCommand = new Command("enable") {
                Description = "Enables one of the current startup programs"
            };

            disableCommand.AddAlias("e");

            disableCommand.AddArgument(new Argument<string>("name"));

            disableCommand.Handler = CommandHandler.Create<string>(EnableCommand.Run);

            return disableCommand;
        }
    }
}