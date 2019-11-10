using System;
using System.Collections.Generic;
using StartupManager.Models;

namespace StartupManager.Services.Registries {
    public interface IRegistryService {
        void AddProgramToStartup(StartupProgram program);
        void RemoveProgramFromStartup(StartupList program);
        StateChange ToggleStartupState(StartupList program, bool enable);
        IEnumerable<StartupList> GetStartupPrograms(IEnumerable<StartupState> programStates);
        StartupList? GetStartupByName(string name, IEnumerable<StartupState> programStates);
        StartupList? GetStartupByName(string name);
        StartupList? GetStartupByPredicate(Func<string, bool> predicate, Func<StartupState, bool> disabledPredicate, IEnumerable<StartupState> programStates);
        StartupList? GetStartupByPredicate(Func<string, bool> predicate, Func<StartupState, bool> disabledPredicate);
        IEnumerable<StartupList> GetStartupPrograms();
        IEnumerable<StartupState> GetStartupProgramStates();
    }
}