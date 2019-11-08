using System;
using System.Linq;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters;
using StartupManager.Helpers;

namespace StartupManager.Commands.Remove {
    public static class RemoveCommand {
        public static void Run(string name, bool confirm) {
            var program = ListCommandHandler.Run(true).FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (program == null) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Red, "Could not find a program with name ");
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Yellow, name);
                return;
            }

            if (program.RequireAdministrator && !WindowsIdentityHelper.IsElevated()) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Red, $"To modify settings for ");
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, " you need to run the command with administrator (sudo)");
                return;
            }

            if (!confirm) {
                var message = $"Are you sure you want to remove '{program.Name}' y/n: ";
                Console.Write(message);
                var confirmation = ConsoleStepWizard.PromptUserForBool("y", "n", message);
                if (!confirmation) {
                    return;
                }
            }

            switch (program.Type) {
                case ListProgram.StartupType.TaskScheduler:
                    TaskSchedulerHelper.RemoveProgramFromStartup(program.Name);
                    break;
                case ListProgram.StartupType.Regedit:
                case ListProgram.StartupType.Shortcut:
                    RegistryHelper.DeleteRegistryKey(program);
                    break;
            }
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
            Console.WriteLine(" has been removed");
        }
    }
}