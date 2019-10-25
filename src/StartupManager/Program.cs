using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using StartupManager.Commands;
using StartupManager.Commands.List;

namespace StartupManager {
    class Program {
        async static Task<int> Main(string[] args) {
            var rootCommand = CommandBuilder.GetRootCommand();
            DisableCommandHandler.Run("Spotify");
            return await rootCommand.InvokeAsync(args);
        }
    }
}