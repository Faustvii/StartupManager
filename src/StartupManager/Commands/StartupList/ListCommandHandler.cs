using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using StartupManager.Helpers;

namespace StartupManager.Commands.StartupList {
    public static class ListCommandHandler {
        private const string StartupRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string DisabledStartupRegistryItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private const string DisabledStartupFolderItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        private static string[] StartFolderSearchPatterns = new [] { "*.exe", "*.lnk", "*.ps1", "*.cmd" };

        public static IEnumerable<ListPrograms> Run(bool detailed) {
            var startupPrograms = new List<ListPrograms>();

            startupPrograms.AddRange(RegistryStartupPrograms(detailed));
            startupPrograms.AddRange(ShellStartup(detailed));
            return startupPrograms;
        }

        private static IEnumerable<ListPrograms> RegistryStartupPrograms(bool detailed) {
            var programs = new List<ListPrograms>();
            using(var allUsersReg = Registry.LocalMachine.OpenSubKey(StartupRegistryPath))
            using(var allUsersDisabledReg = Registry.LocalMachine.OpenSubKey(DisabledStartupRegistryItems))
            using(var currentUserReg = Registry.CurrentUser.OpenSubKey(StartupRegistryPath))
            using(var currentUserDisabledReg = Registry.CurrentUser.OpenSubKey(DisabledStartupRegistryItems)) {
                var currentUserValues = currentUserReg.GetValueNames();

                var currentUserStartups = currentUserValues.Select(x => {
                    var path = currentUserReg.GetValue(x).ToString();
                    var bytes = currentUserDisabledReg.GetValue(x)as byte[];
                    var disabled = CheckIfDisabled(bytes);

                    return new ListPrograms(x, path, false, disabled, DisabledStartupRegistryItems, x);
                }).ToList();

                var allUserReg = allUsersReg.GetValueNames();
                var allUserStartups = allUserReg.Select(x => {
                    var path = allUsersReg.GetValue(x).ToString();
                    var bytes = allUsersDisabledReg.GetValue(x)as byte[];
                    var disabled = CheckIfDisabled(bytes);

                    return new ListPrograms(x, path, true, disabled, DisabledStartupRegistryItems, x);
                }).ToList();
                return currentUserStartups.Concat(allUserStartups);
            }
        }

        private static IEnumerable<ListPrograms> ShellStartup(bool detailed) {
            var currentUser = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var allUsers = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
            using(var allUsersDisabledReg = Registry.LocalMachine.OpenSubKey(DisabledStartupFolderItems))
            using(var currentUserDisabledReg = Registry.CurrentUser.OpenSubKey(DisabledStartupFolderItems)) {
                var currentUserStartups = MyDirectoryExplorer.GetFiles(currentUser, StartFolderSearchPatterns).Select(x => {
                    var fileName = Path.GetFileName(x);
                    var bytes = currentUserDisabledReg.GetValue(fileName)as byte[];
                    var disabled = CheckIfDisabled(bytes);

                    return new ListPrograms(Path.GetFileNameWithoutExtension(x), x, false, disabled, DisabledStartupFolderItems, fileName);
                });
                var allUserStartups = MyDirectoryExplorer.GetFiles(allUsers, StartFolderSearchPatterns).Select(x => {
                    var fileName = Path.GetFileName(x);
                    var bytes = allUsersDisabledReg.GetValue(fileName)as byte[];
                    var disabled = CheckIfDisabled(bytes);

                    return new ListPrograms(Path.GetFileNameWithoutExtension(x), x, true, disabled, DisabledStartupFolderItems, fileName);
                });
                return currentUserStartups.Concat(allUserStartups).ToList();
            }
        }

        private static bool CheckIfDisabled(byte[] bytes) {
            var disabled = false;
            if (bytes != null) {
                //If the last 8 bytes are not 0 then it's disabled
                disabled = bytes.Skip(4).Any(x => x != 0x00);
            }

            return disabled;
        }
    }
}