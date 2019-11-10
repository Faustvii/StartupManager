using System;
using System.Collections.Generic;
using Microsoft.Win32.TaskScheduler;
using StartupManager.Models;
using StartupManager.Services.Directories;
using StartupManager.Services.Registries;
using StartupManager.Services.Schedulers;

namespace StartupManager.Services.Startup {
    public class StartupQueryService : IStartupQueryService {
        private static IRegistryService RegistryService = new RegistryService();
        private static IDirectoryService DirectoryService = new DirectoryService();
        private static ITaskSchedulerService TaskSchedulerService = new TaskSchedulerService();
        public StartupList? GetStartupByName(string name, IEnumerable<StartupState> startupStates) {
            var program = RegistryService.GetStartupByName(name, startupStates);
            if (program == null) {
                program = DirectoryService.GetStartupByName(name, startupStates);
            }
            if (program == null) {
                program = TaskSchedulerService.GetStartupByName(name);
            }

            return program;
        }

        public StartupList? GetStartupByName(string name) {
            var startupStates = RegistryService.GetStartupProgramStates();
            return GetStartupByName(name, startupStates);
        }

        public StartupList? GetStartupByPredicate(Func<string, bool> registryPredicate, Func<string, bool> directoryPredicate, Func<Task, bool> taskSchedulerPredicate, Func<StartupState, bool> disabledPredicate, IEnumerable<StartupState> startupStates) {
            var program = RegistryService.GetStartupByPredicate(registryPredicate, disabledPredicate, startupStates);
            if (program == null) {
                program = DirectoryService.GetStartupByPredicate(directoryPredicate, disabledPredicate, startupStates);
            }
            if (program == null) {
                program = TaskSchedulerService.GetStartupByPredicate(taskSchedulerPredicate);
            }
            return program;
        }

        public StartupList? GetStartupByPredicate(Func<string, bool> registryPredicate, Func<string, bool> directoryPredicate, Func<Task, bool> taskSchedulerPredicate, Func<StartupState, bool> disabledPredicate) {
            var startupStates = RegistryService.GetStartupProgramStates();
            return GetStartupByPredicate(registryPredicate, directoryPredicate, taskSchedulerPredicate, disabledPredicate, startupStates);
        }
    }
}