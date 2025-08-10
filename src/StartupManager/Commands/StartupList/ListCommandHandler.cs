using System;
using System.Collections.Generic;
using System.Linq;
using StartupManager.Models;
using StartupManager.Services.Directories;
using StartupManager.Services.Registries;
using StartupManager.Services.Schedulers;

namespace StartupManager.Commands.StartupList {
    public static class ListCommandHandler {
        private const string DisabledStartupFolderItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        private const string UnauthorizedMessage = "Was unable to get all startup programs (Access denied), try running as administrator";
        private static string[] StartFolderSearchPatterns = new [] { "*.exe", "*.lnk", "*.ps1", "*.cmd" };
        private static IRegistryService RegistryService = new RegistryService();
        private static IDirectoryService DirectoryService = new DirectoryService();
        private static ITaskSchedulerService TaskSchedulerService = new TaskSchedulerService();

        public static(IEnumerable<Models.StartupList> Programs, IEnumerable<ConsoleColorOutput> Messages) Run() {
            var startupPrograms = new List<Models.StartupList>();
            var messages = new List<ConsoleColorOutput>();
            try {
                var startupStates = RegistryService.GetStartupProgramStates();

                var registryStartups = RegistryService.GetStartupPrograms(startupStates);
                if (registryStartups != null)
                    startupPrograms.AddRange(registryStartups);

                var shellStartups = DirectoryService.GetStartupPrograms(startupStates);
                if (shellStartups != null)
                    startupPrograms.AddRange(shellStartups);

                var taskSchedulerStartups = TaskSchedulerService.GetStartupPrograms(false);
                if (taskSchedulerStartups != null)
                    startupPrograms.AddRange(taskSchedulerStartups);

            } catch (UnauthorizedAccessException) {
                messages.Add(new ConsoleColorOutput(WriteMode.Writeline, UnauthorizedMessage, ConsoleColor.Red));
            }

            var sortedAndIndexedPrograms = startupPrograms
                .OrderBy(program => program.Name)
                .Select((program, index) => {
                    program.Index = index + 1; // Assigning index starting from 1
                    return program;
                })
                .ToList();
            return (sortedAndIndexedPrograms, messages);
        }
    }
}