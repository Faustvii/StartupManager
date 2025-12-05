using System.CommandLine;
using System.Threading.Tasks;
using StartupManager.Commands;
using StartupManager.UI;

namespace StartupManager {
    class Program {
        async static Task<int> Main(string[] args) {
            // Show interactive UI if no arguments provided
            if (args == null || args.Length == 0) {
                InteractiveMenu.Show();
                return 0;
            }

            var rootCommand = CommandBuilder.GetRootCommand();
            return await rootCommand.InvokeAsync(args);
        }
    }
}