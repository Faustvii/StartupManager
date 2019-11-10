using System;
using System.Collections.Generic;
using System.Linq;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters;
using StartupManager.Models;
using StartupManager.Services.Identity;
using StartupManager.Services.Registries;
using StartupManager.Services.Schedulers;

namespace StartupManager.Commands.Add {
    public static class AddCommandHandler {
        private static ITaskSchedulerService TaskSchedulerService = new TaskSchedulerService();
        private static IWindowsIdentityService WindowsIdentityService = new WindowsIdentityService();
        private static IRegistryService RegistryService = new RegistryService();
        public static IEnumerable<ConsoleColorOutput> Run(StartupProgram program) {
            var existingPrograms = ListCommandHandler.Run().Programs.ToList();
            var programAlreadyStarts = existingPrograms.FirstOrDefault(x => x.Path.Contains(program.File.FullName, StringComparison.OrdinalIgnoreCase) && !x.Disabled);
            var nameInUseBy = existingPrograms.FirstOrDefault(x => x.Name.Equals(program.Name, StringComparison.OrdinalIgnoreCase));

            if (nameInUseBy != null) {
                return new [] {
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                new ConsoleColorOutput(WriteMode.Writeline, " is already in use, please try again with a different name", ConsoleColor.Red),
                };
            }

            if (programAlreadyStarts != null) {
                var messages = new [] {
                new ConsoleColorOutput(WriteMode.Write, program.File.Name, ConsoleColor.Yellow),
                new ConsoleColorOutput(WriteMode.Writeline, " already starts with windows", ConsoleColor.Red),
                new ConsoleColorOutput(WriteMode.Write, "Want to add another instance of it to startup? y/n: "),
                };
                ConsoleOutputWriter.WriteToConsole(messages);

                var userWantsToContinue = ConsoleStepWizard.PromptUserForBool("y", "n", $"Want to add another? y/n: ");
                if (!userWantsToContinue) {
                    return new List<ConsoleColorOutput>();
                }
            }

            if (program.AllUsers || program.Administrator) {
                if (!WindowsIdentityService.IsElevated()) {
                    return new [] {
                        new ConsoleColorOutput(WriteMode.Writeline, "This requires you run the command as administrator", ConsoleColor.Red)
                    };
                }
            }

            //Current user only programs requires a schedule task to run as Administrator
            if (program.Administrator && !program.AllUsers) {
                TaskSchedulerService.AddProgramToStartup(program);
            } else {
                RegistryService.AddProgramToStartup(program);
            }
            return new [] {
                new ConsoleColorOutput(WriteMode.Write, "Added "),
                    new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, " to startup")
            };
        }
    }
}