using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters;
using StartupManager.Helpers;

namespace StartupManager.Commands.Add {
    public static class AddCommand {
        private static string NameId = "Name";
        private static string PathId = "Path";
        private static string ArgumentsId = "Arguments";
        private static string AdministratorId = "Administrator";
        private static string AllUserId = "All Users";
        public static void Run(string? name, FileInfo? path, string? arguments, bool? admin, bool? allUsers) {
            if (name == null || (path == null || !path.Exists) || arguments == null || admin == null || allUsers == null) {
                var steps = GetWizardSteps(name, path, arguments, admin, allUsers);
                steps = ConsoleStepWizard.UserWizard("Let's guide you through settings up a new startup program", steps);
                System.Console.WriteLine();

                var startupProgram = ParseUserInfo(steps, name, path, arguments, admin, allUsers);
                ValidateInformationWithUser(startupProgram);
                System.Console.WriteLine();
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Green, "Does this look correct? y/n: ");

                var correct = ConsoleStepWizard.PromptUserForBool("y", "n", "Does this look correct? y/n: ");
                if (correct) {
                    AddStartupProgram(startupProgram);
                } else {
                    System.Console.WriteLine("Sorry to hear that, please try again");
                }
            } else {
                var startupProgram = ParseUserInfo(new List<ConsoleStep>(), name, path, arguments, admin, allUsers);
                AddStartupProgram(startupProgram);
            }
        }

        private static void AddStartupProgram(StartupProgram program) {
            var existingPrograms = ListCommandHandler.Run(true).ToList();
            var nameInUseBy = existingPrograms.FirstOrDefault(x => x.RegistryName.Equals(program.Name, StringComparison.OrdinalIgnoreCase));
            var programAlreadyStarts = existingPrograms.FirstOrDefault(x => x.Path.Contains(program.File.FullName, StringComparison.OrdinalIgnoreCase) && !x.Disabled);
            if (nameInUseBy != null) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, " is already in use, please try again with a different name");
                return;
            }
            if (programAlreadyStarts != null) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.File.Name);
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, " already starts with windows");
                Console.Write("Want to add another instance of it to startup? y/n: ");
                var userWantsToContinue = ConsoleStepWizard.PromptUserForBool("y", "n", $"Want to add another? y/n: ");
                if (!userWantsToContinue) {
                    return;
                }
            }

            if (program.AllUsers || program.Administrator) {
                if (!WindowsIdentityHelper.IsElevated()) {
                    System.Console.WriteLine("This requires you run the command as administrator");
                    return;
                }
            }

            //Current user only programs requires a schedule task to run as Administrator
            if (program.Administrator && !program.AllUsers) {
                var taskDef = TaskSchedulerHelper.AddProgramToStartup(program);
                TaskSchedulerHelper.RegisterTask(taskDef, program);
            } else {
                using(var registryKey = RegistryHelper.GetWriteStartupRegistryKey(program)) {
                    registryKey.SetValue(program.Name, $"\"{program.File.FullName}\" {program.Arguments}");
                }
            }

            Console.Write("Added ");
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
            Console.WriteLine(" to startup");
        }

        private static void ValidateInformationWithUser(StartupProgram data) {
            Console.Write($"{NameId}: ");
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, $"{data.Name}");
            Console.Write($"{PathId}: ");
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, $"{data.File}");
            Console.Write($"{ArgumentsId}: ");
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, $"{data.Arguments}");
            Console.Write($"{AdministratorId}: ");
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, $"{data.Administrator}");
            Console.Write($"{AllUserId}: ");
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, $"{data.AllUsers}");
        }

        private static IEnumerable<ConsoleStep> GetWizardSteps(string? name, FileInfo? file, string? arguments, bool? admin, bool? allUsers) {
            var steps = new List<ConsoleStep>();
            if (name == null || string.IsNullOrWhiteSpace(name)) {
                steps.Add(new ConsoleStep(NameId, "What's the name of the program?: ", string.Empty));
            }
            if (file == null || !file.Exists) {
                steps.Add(new ConsoleStep(PathId, "What's the path to the program?: ", new FileInfo("program")));
            }
            if (arguments == null) {
                steps.Add(new ConsoleStep(ArgumentsId, "What's the arguments for the program?: ", string.Empty));
            }
            if (admin == null) {
                steps.Add(new ConsoleStep(AdministratorId, "Do you want to run this program as an Administrator? y/n: ", false));
            }
            if (allUsers == null) {
                steps.Add(new ConsoleStep(AllUserId, "Do you want to run this program for all users? y/n: ", false));
            }
            return steps;
        }

        private static StartupProgram ParseUserInfo(IEnumerable<ConsoleStep> steps, string? name, FileInfo? file, string? arguments, bool? admin, bool? allUsers) {
            var nameVal = name ?? (string)steps.Single(x => x.Id == NameId).UserValue;
            var fileVal = file ?? (FileInfo)steps.Single(x => x.Id == PathId).UserValue;
            if (!fileVal.Exists) {
                fileVal = (FileInfo)steps.Single(x => x.Id == PathId).UserValue;
            }
            var argumentsVal = arguments ?? (string)steps.Single(x => x.Id == ArgumentsId).UserValue;
            var adminVal = admin ?? (bool)steps.Single(x => x.Id == AdministratorId).UserValue;
            var allUserVal = allUsers ?? (bool)steps.Single(x => x.Id == AllUserId).UserValue;
            return new StartupProgram(nameVal, fileVal, argumentsVal, adminVal, allUserVal);
        }
    }
}