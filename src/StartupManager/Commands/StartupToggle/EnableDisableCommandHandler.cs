using System;
using System.Collections.Generic;
using StartupManager.Models;
using StartupManager.Services.Directories;
using StartupManager.Services.Registries;
using StartupManager.Services.Schedulers;
using StartupManager.Services.Startup;

namespace StartupManager.Commands.StartupToggle {
    public static class EnableDisableCommandHandler {
        private static IRegistryService RegistryService = new RegistryService();
        private static IDirectoryService DirectoryService = new DirectoryService();
        private static ITaskSchedulerService TaskSchedulerService = new TaskSchedulerService();
        private static IStartupQueryService StartupQueryService = new StartupQueryService();
        public static IEnumerable<ConsoleColorOutput> Run(string name, bool enable) {
            var consoleMessages = new List<ConsoleColorOutput>();
            var toggleText = enable ? "enabled" : "disabled";
            var startupStates = RegistryService.GetStartupProgramStates();
            var program = StartupQueryService.GetStartupByName(name, startupStates);

            if (program == null) {
                return WriteProgramNotFoundConsoleOutput(name);
            }

            switch (program.Type) {
                case Models.StartupList.StartupType.TaskScheduler:
                    consoleMessages.AddRange(ToggleThroughTaskScheduler(enable, toggleText, program));
                    break;
                case Models.StartupList.StartupType.Regedit:
                case Models.StartupList.StartupType.Shortcut:
                    consoleMessages.AddRange(ToggleThroughRegedit(enable, toggleText, program));
                    break;
            }
            return consoleMessages;
        }

        private static IEnumerable<ConsoleColorOutput> ToggleThroughTaskScheduler(bool enable, string toggleText, Models.StartupList program) {
            var result = TaskSchedulerService.ToggleStartupState(program, enable);
            switch (result) {
                case StateChange.SameState:
                    return WriteSameStateConsoleOutput(toggleText, program);
                case StateChange.Success:
                    return WriteToggledConsoleOutput(toggleText, program);
                case StateChange.Unauthorized:
                    return WriteRequireAdministratorConsoleOutput(program);
            }
            return new List<ConsoleColorOutput>();
        }

        private static IEnumerable<ConsoleColorOutput> ToggleThroughRegedit(bool enable, string toggleText, Models.StartupList program) {
            var result = RegistryService.ToggleStartupState(program, enable);
            switch (result) {
                case Models.StateChange.SameState:
                    return WriteSameStateConsoleOutput(toggleText, program);
                case Models.StateChange.Success:
                    return WriteToggledConsoleOutput(toggleText, program);
                case Models.StateChange.Unauthorized:
                    return WriteRequireAdministratorConsoleOutput(program);
            }
            return new List<ConsoleColorOutput>();
        }

        private static IEnumerable<ConsoleColorOutput> WriteProgramNotFoundConsoleOutput(string name) {
            return new [] {
                new ConsoleColorOutput(WriteMode.Write, "Could not find a program with name ", ConsoleColor.Red),
                    new ConsoleColorOutput(WriteMode.Writeline, name, ConsoleColor.Yellow),
            };
        }

        private static IEnumerable<ConsoleColorOutput> WriteSameStateConsoleOutput(string toggleText, Models.StartupList program) {
            return new [] {
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, $" is already {toggleText}"),
            };
        }

        private static IEnumerable<ConsoleColorOutput> WriteRequireAdministratorConsoleOutput(Models.StartupList program) {
            return new [] {
                new ConsoleColorOutput(WriteMode.Write, $"To modify settings for ", ConsoleColor.Red),
                    new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, " you need to run the command with administrator (sudo)", ConsoleColor.Red),
            };
        }

        private static IEnumerable<ConsoleColorOutput> WriteToggledConsoleOutput(string toggleText, Models.StartupList program) {
            return new [] {
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, $" has been {toggleText}"),
            };
        }
    }
}