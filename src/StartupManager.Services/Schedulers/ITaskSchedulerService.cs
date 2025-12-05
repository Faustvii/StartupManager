using System;
using System.Collections.Generic;
using Microsoft.Win32.TaskScheduler;
using StartupManager.Models;

namespace StartupManager.Services.Schedulers {
    public interface ITaskSchedulerService {
        Microsoft.Win32.TaskScheduler.Task AddProgramToStartup(StartupProgram program);
        void RemoveProgramFromStartup(string name);
        IEnumerable<StartupList> GetStartupPrograms(bool includeWindows);
        StartupList? GetStartupByName(string name);
        StartupList? GetStartupByPredicate(Func<Task, bool> predicate);
        StateChange ToggleStartupState(StartupList program, bool enable);
        StateChange SetPriority(StartupList program, ProcessPriority priority);
    }
}