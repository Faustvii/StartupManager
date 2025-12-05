using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StartupManager.ConsoleOutputters;
using StartupManager.Models;

namespace StartupManager.Commands.Add {
    public static class AddCommand {
        private static string NameId = "Name";
        private static string PathId = "Path";
        private static string ArgumentsId = "Arguments";
        private static string AdministratorId = "Administrator";
        private static string AllUserId = "All Users";
        private static string PriorityId = "Priority";

        public static void Run(string? name, FileInfo? path, string? arguments, bool? admin, bool? allUsers, string? priority) {
            if (name == null || (path == null || !path.Exists) || arguments == null || admin == null || allUsers == null || priority == null) {
                var steps = GetWizardSteps(name, path, arguments, admin, allUsers, priority);
                steps = ConsoleStepWizard.UserWizard("Let's guide you through settings up a new startup program", steps);
                System.Console.WriteLine();

                var startupProgram = ParseUserInfo(steps, name, path, arguments, admin, allUsers, priority);
                ValidateInformationWithUser(startupProgram);
                System.Console.WriteLine();
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Green, "Does this look correct? y/n: ");

                var correct = ConsoleStepWizard.PromptUserForBool("y", "n", "Does this look correct? y/n: ");
                if (correct) {
                    ExecuteHandler(startupProgram);
                } else {
                    System.Console.WriteLine("Sorry to hear that, please try again");
                }
            } else {
                var startupProgram = ParseUserInfo(new List<ConsoleStep>(), name, path, arguments, admin, allUsers, priority);
                ExecuteHandler(startupProgram);
            }
        }

        private static void ExecuteHandler(StartupProgram startupProgram) {
            var messages = AddCommandHandler.Run(startupProgram);
            ConsoleOutputWriter.WriteToConsole(messages);
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
            Console.Write($"{PriorityId}: ");
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, $"{data.Priority}");
        }

        private static IEnumerable<ConsoleStep> GetWizardSteps(string? name, FileInfo? file, string? arguments, bool? admin, bool? allUsers, string? priority) {
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
            if (priority == null) {
                steps.Add(new ConsoleStep(PriorityId, "What priority should this program run at? (Idle/BelowNormal/Normal/AboveNormal/High/Realtime) [Normal]: ", "Normal"));
            }
            return steps;
        }

        private static StartupProgram ParseUserInfo(IEnumerable<ConsoleStep> steps, string? name, FileInfo? file, string? arguments, bool? admin, bool? allUsers, string? priority) {
            var nameVal = name ?? (string) steps.Single(x => x.Id == NameId).UserValue;
            var fileVal = file ?? (FileInfo) steps.Single(x => x.Id == PathId).UserValue;
            if (!fileVal.Exists) {
                fileVal = (FileInfo) steps.Single(x => x.Id == PathId).UserValue;
            }
            var argumentsVal = arguments ?? (string) steps.Single(x => x.Id == ArgumentsId).UserValue;
            var adminVal = admin ?? (bool) steps.Single(x => x.Id == AdministratorId).UserValue;
            var allUserVal = allUsers ?? (bool) steps.Single(x => x.Id == AllUserId).UserValue;
            var priorityStr = priority ?? (string) steps.Single(x => x.Id == PriorityId).UserValue;
            var priorityVal = ParsePriority(priorityStr);
            return new StartupProgram(nameVal, fileVal, argumentsVal, adminVal, allUserVal, priorityVal);
        }

        private static ProcessPriority ParsePriority(string priority) {
            if (Enum.TryParse<ProcessPriority>(priority, true, out var result)) {
                return result;
            }
            return ProcessPriority.Normal;
        }
    }
}