using System;
using System.Linq;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters;
using StartupManager.Helpers;

namespace StartupManager.Commands.StartupToggle {
    public static class EnableDisableCommandHandler {
        public static void Run(string name, bool enable) {
            var toggleText = enable ? "enabled" : "disabled";
            var program = ListCommandHandler.Run(true).FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (program == null) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Red, "Could not find a program with name ");
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Yellow, name);
                return;
            }

            try {
                using(var reg = RegistryHelper.GetWriteRegistryKey(program)) {
                    byte[] bytes;
                    if (enable) {
                        bytes = RegistryHelper.MakeEnabledBytes();
                    } else {
                        bytes = RegistryHelper.MakeDisabledBytes();
                    }
                    RegistryHelper.SetBytes(reg, program.RegistryName, bytes);

                    ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
                    System.Console.WriteLine($" has been {toggleText}");
                }
            } catch (UnauthorizedAccessException) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Red, $"To modify settings for ");
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, " you need to run the command with administrator (sudo)");
            }
        }
    }
}