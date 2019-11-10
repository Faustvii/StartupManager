using System;
using System.Collections.Generic;
using System.Linq;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters;
using StartupManager.Models;
using StartupManager.Services.Directories;
using StartupManager.Services.Identity;
using StartupManager.Services.Registries;
using StartupManager.Services.Schedulers;

namespace StartupManager.Commands.Remove {
    public static class RemoveCommandHandler {
        private static ITaskSchedulerService TaskSchedulerService = new TaskSchedulerService();
        private static IWindowsIdentityService WindowsIdentityService = new WindowsIdentityService();
        private static IDirectoryService DirectoryService = new DirectoryService();
        private static IRegistryService RegistryService = new RegistryService();
        public static IEnumerable<ConsoleColorOutput> Run(string name, bool confirm) {
            var program = ListCommandHandler.Run().Programs.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (program == null) {
                return new [] {
                new ConsoleColorOutput(WriteMode.Write, "Could not find a program with name ", ConsoleColor.Red),
                new ConsoleColorOutput(WriteMode.Writeline, name, ConsoleColor.Yellow),
                };
            }

            if (program.RequireAdministrator && !WindowsIdentityService.IsElevated()) {
                return new [] {
                    new ConsoleColorOutput(WriteMode.Write, $"To modify settings for ", ConsoleColor.Red),
                        new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                        new ConsoleColorOutput(WriteMode.Writeline, " you need to run the command with administrator (sudo)", ConsoleColor.Red),
                };
            }

            if (!confirm) {
                var message = $"Are you sure you want to remove '{program.Name}' y/n: ";
                var messages = new [] {
                    new ConsoleColorOutput(WriteMode.Write, message),
                };
                ConsoleOutputWriter.WriteToConsole(messages);
                var confirmation = ConsoleStepWizard.PromptUserForBool("y", "n", message);
                if (!confirmation) {
                    return new List<ConsoleColorOutput>();
                }
            }

            switch (program.Type) {
                case Models.StartupList.StartupType.TaskScheduler:
                    TaskSchedulerService.RemoveProgramFromStartup(program.Name);
                    break;
                case Models.StartupList.StartupType.Shortcut:
                    DirectoryService.RemoveProgramFromStartup(program);
                    break;
                case Models.StartupList.StartupType.Regedit:
                    RegistryService.RemoveProgramFromStartup(program);
                    break;
            }
            return new [] {
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, " has been removed"),
            };
        }
    }
}