using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StartupManager.Models;
using StartupManager.Services.Registries;

namespace StartupManager.Services.Directories {
    public class DirectoryService : IDirectoryService {
        private const string DisabledStartupFolderItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        private static string[] ExecuteableSearchPatterns = new [] { "*.exe", "*.lnk", "*.ps1", "*.cmd" };
        private static IRegistryService RegistryService = new RegistryService();
        private static string CurrentUserStartup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private static string AllUsersStartup = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        public StartupList? GetStartupByPredicate(Func<string, bool> filePredicate, Func<StartupState, bool> disabledPredicate, IEnumerable<StartupState> startupStates) {
            var files = GetFiles(CurrentUserStartup, ExecuteableSearchPatterns);
            var path = files.FirstOrDefault(filePredicate);
            if (path == null) {
                files = GetFiles(AllUsersStartup, ExecuteableSearchPatterns);
                path = files.FirstOrDefault(filePredicate);
                if (path == null) {
                    return null;
                }
                var fileName = Path.GetFileName(path);
                var parsedName = Path.GetFileNameWithoutExtension(path);
                var disabled = startupStates.Any(disabledPredicate);

                return new StartupList(parsedName, path, requireAdministrator : true, disabled, StartupList.StartupType.Shortcut, allUsers : true, string.Empty, DisabledStartupFolderItems, fileName);
            } else {
                var parsedName = Path.GetFileNameWithoutExtension(path);
                var fileName = Path.GetFileName(path);
                var disabled = startupStates.Any(disabledPredicate);
                return new StartupList(parsedName, path, requireAdministrator : false, disabled, StartupList.StartupType.Shortcut, allUsers : false, string.Empty, DisabledStartupFolderItems, fileName);
            }
        }

        public StartupList? GetStartupByPredicate(Func<string, bool> predicate, Func<StartupState, bool> disabledPredicate) {
            var startupStates = RegistryService.GetStartupProgramStates();
            return GetStartupByPredicate(predicate, disabledPredicate, startupStates);
        }

        public StartupList? GetStartupByName(string name, IEnumerable<StartupState> startupStates) {
            Func<string, bool> namePredicate = x => Path.GetFileNameWithoutExtension(x).Equals(name, StringComparison.OrdinalIgnoreCase);
            Func<StartupState, bool> disabledPredicate = x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Disabled;
            return GetStartupByPredicate(namePredicate, disabledPredicate, startupStates);
        }

        public StartupList? GetStartupByName(string name) {
            var startupStates = RegistryService.GetStartupProgramStates();
            return GetStartupByName(name, startupStates);
        }

        public IEnumerable<StartupList> GetStartupPrograms() {
            var startupStates = RegistryService.GetStartupProgramStates();
            return GetStartupPrograms(startupStates);
        }

        public IEnumerable<StartupList> GetStartupPrograms(IEnumerable<StartupState> startupStates) {
            var programs = new List<StartupList>();

            var currentUserStartups = GetShellStartup(allUsers: false, CurrentUserStartup, startupStates);
            programs.AddRange(currentUserStartups);

            var allUserStartups = GetShellStartup(allUsers: true, AllUsersStartup, startupStates);
            programs.AddRange(allUserStartups);

            return programs;
        }

        private static IEnumerable<StartupList> GetShellStartup(bool allUsers, string path, IEnumerable<StartupState> startupStates) {
            var currentStartups = GetFiles(path, ExecuteableSearchPatterns).Select(name => {
                var fileName = Path.GetFileName(name);
                var disabled = startupStates.Any(x => x.Name.Equals(fileName, StringComparison.Ordinal) && x.Disabled);

                return new StartupList(Path.GetFileNameWithoutExtension(name), name, requireAdministrator : allUsers, disabled, StartupList.StartupType.Shortcut, allUsers : allUsers, DisabledStartupFolderItems, string.Empty, fileName);
            });
            return currentStartups.ToList();
        }

        public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly) {
            return searchPatterns.SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        public void RemoveProgramFromStartup(StartupList program) {
            File.Delete(program.Path);
        }
    }
}