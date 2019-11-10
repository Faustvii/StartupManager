using System;
using System.Collections.Generic;
using StartupManager.Models;

namespace StartupManager.Services.Directories {
    public interface IDirectoryService {
        void RemoveProgramFromStartup(StartupList program);
        IEnumerable<StartupList> GetStartupPrograms();
        IEnumerable<StartupList> GetStartupPrograms(IEnumerable<StartupState> startupStates);
        StartupList? GetStartupByName(string name, IEnumerable<StartupState> startupStates);
        StartupList? GetStartupByName(string name);
        StartupList? GetStartupByPredicate(Func<string, bool> filePredicate, Func<StartupState, bool> disabledPredicate, IEnumerable<StartupState> programStates);
        StartupList? GetStartupByPredicate(Func<string, bool> filePredicate, Func<StartupState, bool> disabledPredicate);
    }
}