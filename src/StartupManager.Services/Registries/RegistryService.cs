using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Win32;
using StartupManager.Models;

namespace StartupManager.Services.Registries {
    public class RegistryService : IRegistryService {
        private static string[] StartupRegistryPaths = new [] {
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            @"Software\Microsoft\Windows\CurrentVersion\RunOnce",
            @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run",
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\RunOnce"
        };
        private static string StartupRegistryPath => @"Software\Microsoft\Windows\CurrentVersion\Run";

        private const string DisabledStartupRegistryItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private const string DisabledStartupFolderItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        private static byte[] EnabledBytes => new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public void AddProgramToStartup(StartupProgram program) {
            using(var key = GetWriteRegistryKey(StartupRegistryPath, program.AllUsers)) {
                key.SetValue(program.Name, $"\"{program.File.FullName}\" {program.Arguments}");
            }
        }

        public void RemoveProgramFromStartup(StartupList program) {
            using(var key = GetWriteRegistryKey(program.RegistryPath, program.AllUsers)) {
                key.DeleteValue(program.RegistryName);
            }
        }

        public StateChange ToggleStartupState(StartupList program, bool enable) {
            try {
                using(var reg = GetWriteRegistryKey(program.DisabledRegistryPath, program.AllUsers)) {
                    byte[] bytes;
                    if (enable) {
                        bytes = EnabledBytes;
                    } else {
                        bytes = MakeDisabledBytes();
                    }

                    var currentValue = (byte[]?) reg.GetValue(program.RegistryName);

                    if (currentValue == null) {
                        reg.SetValue(program.RegistryName, bytes);
                        return StateChange.Success;
                    }

                    var isAlreadyTheRequestState = new ReadOnlySpan<byte>(bytes.Take(4).ToArray()).SequenceEqual(currentValue.Take(4).ToArray());

                    if (isAlreadyTheRequestState) {
                        return StateChange.SameState;
                    } else {
                        reg.SetValue(program.RegistryName, bytes);
                        return StateChange.Success;
                    }
                }
            } catch (UnauthorizedAccessException) {
                return StateChange.Unauthorized;
            }
        }

        public IEnumerable<StartupList> GetStartupPrograms(IEnumerable<StartupState> programStates) {
            var startupPrograms = new List<StartupList>();
            var startupStates = programStates;
            foreach (var path in StartupRegistryPaths) {
                var currentUser = GetStartups(path, allUsers : false, startupStates);
                if (currentUser != null) {
                    startupPrograms.AddRange(currentUser);
                }
                var allUsers = GetStartups(path, allUsers : true, startupStates);
                if (allUsers != null) {
                    startupPrograms.AddRange(allUsers);
                }
            }
            return startupPrograms;
        }

        public IEnumerable<StartupList> GetStartupPrograms() {
            var startupStates = GetStartupProgramStates();
            return GetStartupPrograms(startupStates);
        }

        public StartupList? GetStartupByName(string name, IEnumerable<StartupState> programStates) {
            Func<string, bool> namePredicate = x => x.Equals(name, StringComparison.OrdinalIgnoreCase);
            Func<StartupState, bool> disabledPredicate = x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Disabled;
            return GetStartupByPredicate(namePredicate, disabledPredicate, programStates);
        }

        public StartupList? GetStartupByName(string name) {
            var startupStates = GetStartupProgramStates();
            return GetStartupByName(name, startupStates);
        }

        public StartupList? GetStartupByPredicate(Func<string, bool> predicate, Func<StartupState, bool> disabledPredicate, IEnumerable<StartupState> programStates) {
            var program = FindStartupList(predicate, disabledPredicate, false, programStates);
            if (program == null) {
                program = FindStartupList(predicate, disabledPredicate, true, programStates);
            }
            return program;
        }

        public StartupList? GetStartupByPredicate(Func<string, bool> predicate, Func<StartupState, bool> disabledPredicate) {
            var startupStates = GetStartupProgramStates();
            return GetStartupByPredicate(predicate, disabledPredicate, startupStates);
        }

        private StartupList? FindStartupList(Func<string, bool> predicate, Func<StartupState, bool> disabledPredicate, bool allUsers, IEnumerable<StartupState> startupStates) {
            var startupRegistryKeys = GetReadRegistryKeys(allUsers, StartupRegistryPaths);
            foreach (var registry in startupRegistryKeys) {
                using(registry) {
                    if (registry == null)
                        continue;

                    var startupValues = registry.GetValueNames();
                    var name = startupValues.FirstOrDefault(predicate);
                    if (name != null) {
                        var path = registry?.GetValue(name)?.ToString();
                        var disabled = startupStates.Any(disabledPredicate);
                        return new StartupList(name, path, requireAdministrator : allUsers, disabled, StartupList.StartupType.Regedit, allUsers : allUsers, StartupRegistryPaths.First(x => registry?.Name.Contains(x) == true), DisabledStartupRegistryItems, name);
                    }
                }
            }
            return null;
        }

        public IEnumerable<StartupState> GetStartupProgramStates() {
            var startupStates = new List<StartupState>();
            using(var currentUserDisabledReg = GetReadRegistryKey(DisabledStartupRegistryItems, allUsers : false))
            using(var allUserDisabledReg = GetReadRegistryKey(DisabledStartupRegistryItems, allUsers : true))
            using(var currentUserShellDisabledReg = GetReadRegistryKey(DisabledStartupFolderItems, allUsers : false))
            using(var allUserShellDisabledReg = GetReadRegistryKey(DisabledStartupFolderItems, allUsers : true)) {
                var currentUsers = currentUserDisabledReg?.GetValueNames().Select(x => GetStartupState(currentUserDisabledReg, x));
                var allUsers = allUserDisabledReg?.GetValueNames().Select(x => GetStartupState(allUserDisabledReg, x));
                var currentUsersShell = currentUserShellDisabledReg?.GetValueNames().Select(x => GetStartupState(currentUserShellDisabledReg, x));
                var allUsersShell = allUserShellDisabledReg?.GetValueNames().Select(x => GetStartupState(allUserShellDisabledReg, x));
                if (currentUsers != null) {
                    startupStates.AddRange(currentUsers);
                }
                if (currentUsersShell != null) {
                    startupStates.AddRange(currentUsersShell);
                }
                if (allUsers != null) {
                    startupStates.AddRange(allUsers);
                }
                if (allUsersShell != null) {
                    startupStates.AddRange(allUsersShell);
                }
            }
            return startupStates;
        }

        private IEnumerable<StartupList> GetStartups(string registryPath, bool allUsers, IEnumerable<StartupState> startupStates) {
            var programs = new List<StartupList>();
            var startupRegistryKeys = GetReadRegistryKeys(allUsers, registryPath);
            foreach (var registry in startupRegistryKeys) {
                using(registry) {
                    if (registry == null)
                        continue;

                    var startupValues = registry.GetValueNames();
                    var startupPrograms = startupValues.Select(name => {
                        var path = registry?.GetValue(name)?.ToString();
                        var disabled = startupStates.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Disabled);

                        return new StartupList(name, path, requireAdministrator : allUsers, disabled, StartupList.StartupType.Regedit, allUsers : allUsers, StartupRegistryPaths.First(x => registry?.Name.Contains(x) == true), DisabledStartupRegistryItems, name);
                    }).Where(x => !string.IsNullOrWhiteSpace(x.Path)).ToList();
                    programs.AddRange(startupPrograms);
                }
            }
            return programs;
        }

        private static RegistryKey GetWriteRegistryKey(string registryPath, bool allUsers) {
            if (allUsers) {
                return Registry.LocalMachine.CreateSubKey(registryPath);
            } else {
                return Registry.CurrentUser.CreateSubKey(registryPath);
            }
        }

        private static RegistryKey? GetReadRegistryKey(string registryPath, bool allUsers) {
            if (allUsers) {
                return Registry.LocalMachine.OpenSubKey(registryPath);
            } else {
                return Registry.CurrentUser.OpenSubKey(registryPath);
            }
        }

        private static void DeleteRegistryKey(StartupList program) {
            using(var key = GetWriteRegistryKey(program.RegistryPath, program.AllUsers)) {
                key.DeleteValue(program.RegistryName);
            }
        }

        private static IEnumerable<RegistryKey?> GetReadRegistryKeys(bool allUsers, params string[] registryKeys) {
            if (allUsers) {
                return registryKeys.Select(x => Registry.LocalMachine.OpenSubKey(x));
            } else {
                return registryKeys.Select(x => Registry.CurrentUser.OpenSubKey(x));
            }
        }

        private StartupState GetStartupState(RegistryKey disabledReg, string name) {
            var bytes = disabledReg.GetValue(name) as byte[];
            var disabled = CheckIfDisabled(bytes);
            return new StartupState(name, disabled);
        }

        private static bool CheckIfDisabled([AllowNull] byte[] bytes) {
            var disabled = false;
            if (bytes != null) {
                //If the last 8 bytes are not 0 then it's disabled
                disabled = bytes.Skip(4).Any(x => x != 0x00);
            }

            return disabled;
        }

        private static byte[] MakeDisabledBytes() {
            var startBytes = new byte[] { 0x03, 0x00, 0x00, 0x00 };
            var now = DateTime.Now.Ticks;
            var timeBytes = BitConverter.GetBytes(now);
            var bytes = startBytes.Concat(timeBytes).ToArray();
            return bytes;
        }
    }
}