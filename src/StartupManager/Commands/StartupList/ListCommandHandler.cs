using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using StartupManager.ConsoleOutputters;
using StartupManager.Helpers;

namespace StartupManager.Commands.StartupList {
    public static class ListCommandHandler {
        private static string[] StartupRegistryPaths = new [] {
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            @"Software\Microsoft\Windows\CurrentVersion\RunOnce",
            @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run",
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\RunOnce"
        };
        private const string DisabledStartupRegistryItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private const string DisabledStartupFolderItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        private const string UnauthorizedMessage = "Was unable to get all startup programs (Access denied), try running as administrator";
        private static string[] StartFolderSearchPatterns = new [] { "*.exe", "*.lnk", "*.ps1", "*.cmd" };

        public static IEnumerable<ListProgram> Run(bool detailed) {
            var startupPrograms = new List<ListProgram>();

            var registryStartups = RegistryStartupProgramsNew(detailed);
            var shellStartups = ShellStartup(detailed);
            var taskSchedulerStartups = TaskSchedulerHelper.GetStartupTaskScheduler(false);
            if (registryStartups != null)
                startupPrograms.AddRange(registryStartups);

            if (shellStartups != null)
                startupPrograms.AddRange(shellStartups);

            if (taskSchedulerStartups != null)
                startupPrograms.AddRange(taskSchedulerStartups);

            return startupPrograms;
        }

        private static IEnumerable<ListProgram> RegistryStartupProgramsNew(bool detailed) {
            var programs = new List<ListProgram>();
            try {
                var userStartups = GetStartupRegistry(allUsers: false);
                programs.AddRange(userStartups);

                var globalStartups = GetStartupRegistry(allUsers: true);
                programs.AddRange(globalStartups);

                return programs;
            } catch (UnauthorizedAccessException) {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, UnauthorizedMessage);
                return programs;
            }
        }

        private static List<ListProgram> GetStartupRegistry(bool allUsers) {
            var programs = new List<ListProgram>();
            using(var disabledReg = RegistryHelper.GetReadRegistryKey(DisabledStartupRegistryItems, allUsers)) {
                var startupRegistryKeys = RegistryHelper.GetReadRegistryKeys(allUsers, StartupRegistryPaths);
                foreach (var registry in startupRegistryKeys)using(registry) {
                    if (registry == null)
                        continue;

                    var startupValues = registry.GetValueNames();
                    var startupPrograms = startupValues.Select(name => {
                        var path = registry.GetValue(name).ToString();
                        var bytes = disabledReg.GetValue(name)as byte[];
                        var disabled = CheckIfDisabled(bytes);

                        return new ListProgram(name, path, requireAdministrator : allUsers, disabled, ListProgram.StartupType.Regedit, allUsers : allUsers, DisabledStartupRegistryItems, name);
                    }).Where(x => !string.IsNullOrWhiteSpace(x.Path)).ToList();
                    programs.AddRange(startupPrograms);
                }
            }
            return programs;
        }

        private static IEnumerable<ListProgram> ShellStartup(bool detailed) {
            var programs = new List<ListProgram>();
            try {
                var currentUser = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                var allUsers = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);

                var currentUserStartups = GetShellStartup(allUsers: false, currentUser);
                programs.AddRange(currentUserStartups);

                var allUserStartups = GetShellStartup(allUsers: true, allUsers);
                programs.AddRange(allUserStartups);

                return programs;
            } catch (UnauthorizedAccessException) {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, UnauthorizedMessage);
                return programs;
            }
        }

        private static IEnumerable<ListProgram> GetShellStartup(bool allUsers, string path) {
            using(var disabledReg = RegistryHelper.GetReadRegistryKey(DisabledStartupFolderItems, allUsers)) {
                var currentStartups = MyDirectoryExplorer.GetFiles(path, StartFolderSearchPatterns).Select(name => {
                    var fileName = Path.GetFileName(name);
                    var bytes = disabledReg.GetValue(fileName)as byte[];
                    var disabled = CheckIfDisabled(bytes);

                    return new ListProgram(Path.GetFileNameWithoutExtension(name), name, requireAdministrator : allUsers, disabled, ListProgram.StartupType.Shortcut, allUsers : allUsers, DisabledStartupFolderItems, fileName);
                });
                return currentStartups.ToList();
            }
        }

        private static bool CheckIfDisabled([AllowNull] byte[] bytes) {
            var disabled = false;
            if (bytes != null) {
                //If the last 8 bytes are not 0 then it's disabled
                disabled = bytes.Skip(4).Any(x => x != 0x00);
            }

            return disabled;
        }
    }
}