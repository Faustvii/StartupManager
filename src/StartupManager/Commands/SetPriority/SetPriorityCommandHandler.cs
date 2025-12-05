using System;
using System.Collections.Generic;
using StartupManager.Models;
using StartupManager.Services.Registries;
using StartupManager.Services.Schedulers;
using StartupManager.Services.Startup;

namespace StartupManager.Commands.SetPriority {
    public static class SetPriorityCommandHandler {
        private static ITaskSchedulerService TaskSchedulerService = new TaskSchedulerService();
        private static IStartupQueryService StartupQueryService = new StartupQueryService();
        private static IRegistryService RegistryService = new RegistryService();

        public static IEnumerable<ConsoleColorOutput> Run(string name, string priority) {
            var consoleMessages = new List<ConsoleColorOutput>();

            if (!Enum.TryParse<ProcessPriority>(priority, true, out var priorityValue)) {
                return new[] {
                    new ConsoleColorOutput(WriteMode.Write, "Invalid priority: ", ConsoleColor.Red),
                    new ConsoleColorOutput(WriteMode.Writeline, priority, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, "Valid priorities are: Idle, BelowNormal, Normal, AboveNormal, High, Realtime", ConsoleColor.Red)
                };
            }

            var startupStates = RegistryService.GetStartupProgramStates();
            Models.StartupList? program = null;
            if (int.TryParse(name, out int index)) {
                program = StartupQueryService.GetStartupByIndex(index);
            }
            program ??= StartupQueryService.GetStartupByName(name, startupStates);

            if (program == null) {
                return WriteProgramNotFoundConsoleOutput(name);
            }

            if (program.Type != Models.StartupList.StartupType.TaskScheduler) {
                return new[] {
                    new ConsoleColorOutput(WriteMode.Write, "Priority can only be set for Task Scheduler startup programs. ", ConsoleColor.Red),
                    new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                    new ConsoleColorOutput(WriteMode.Writeline, $" is a {program.Type} startup program.", ConsoleColor.Red)
                };
            }

            var result = TaskSchedulerService.SetPriority(program, priorityValue);
            switch (result) {
                case StateChange.SameState:
                    return WriteSamePriorityConsoleOutput(program, priorityValue);
                case StateChange.Success:
                    return WritePrioritySetConsoleOutput(program, priorityValue);
                case StateChange.Unauthorized:
                    return WriteRequireAdministratorConsoleOutput(program);
            }
            return new List<ConsoleColorOutput>();
        }

        private static IEnumerable<ConsoleColorOutput> WriteProgramNotFoundConsoleOutput(string name) {
            return new[] {
                new ConsoleColorOutput(WriteMode.Write, "Could not find a program with name/index ", ConsoleColor.Red),
                new ConsoleColorOutput(WriteMode.Writeline, name, ConsoleColor.Yellow),
            };
        }

        private static IEnumerable<ConsoleColorOutput> WriteSamePriorityConsoleOutput(Models.StartupList program, ProcessPriority priority) {
            return new[] {
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                new ConsoleColorOutput(WriteMode.Writeline, $" already has priority set to {priority}"),
            };
        }

        private static IEnumerable<ConsoleColorOutput> WriteRequireAdministratorConsoleOutput(Models.StartupList program) {
            return new[] {
                new ConsoleColorOutput(WriteMode.Write, $"To modify priority for ", ConsoleColor.Red),
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                new ConsoleColorOutput(WriteMode.Writeline, " you need to run the command with administrator (sudo)", ConsoleColor.Red),
            };
        }

        private static IEnumerable<ConsoleColorOutput> WritePrioritySetConsoleOutput(Models.StartupList program, ProcessPriority priority) {
            return new[] {
                new ConsoleColorOutput(WriteMode.Write, program.Name, ConsoleColor.Yellow),
                new ConsoleColorOutput(WriteMode.Writeline, $" priority has been set to {priority}"),
            };
        }
    }
}

