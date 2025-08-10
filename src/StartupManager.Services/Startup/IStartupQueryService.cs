using System;
using System.Collections.Generic;
using StartupManager.Models;

namespace StartupManager.Services.Startup {
    public interface IStartupQueryService {
        StartupList? GetStartupByName(string name, IEnumerable<StartupState> startupStates);
        StartupList? GetStartupByName(string name);
        StartupList? GetStartupByIndex(int index, IEnumerable<StartupState> startupStates);
        StartupList? GetStartupByIndex(int index);
        StartupList? GetStartupByPredicate(Func<string, bool> registryPredicate, Func<string, bool> directoryPredicate, Func<Microsoft.Win32.TaskScheduler.Task, bool> taskSchedulerPredicate, Func<StartupState, bool> disabledPredicate, IEnumerable<StartupState> startupStates);
        StartupList? GetStartupByPredicate(Func<string, bool> registryPredicate, Func<string, bool> directoryPredicate, Func<Microsoft.Win32.TaskScheduler.Task, bool> taskSchedulerPredicate, Func<StartupState, bool> disabledPredicate);
    }
}