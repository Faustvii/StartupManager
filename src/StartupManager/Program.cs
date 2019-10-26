using System.CommandLine.Invocation;
using System.Threading.Tasks;
using StartupManager.Commands;

namespace StartupManager {
    class Program {
        async static Task<int> Main(string[] args) {
            var rootCommand = CommandBuilder.GetRootCommand();
            return await rootCommand.InvokeAsync(args);
        }
    }
}