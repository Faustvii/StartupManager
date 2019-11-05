using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using StartupManager.Commands.Add;
using StartupManager.Commands.StartupList;

namespace StartupManager.Helpers {
    public static class RegistryHelper {
        private static string StartupRegistryPath => @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static RegistryKey GetWriteRegistryKey(ListProgram program) {
            if (program.AllUsers) {
                return Registry.LocalMachine.CreateSubKey(program.RegistryPath);
            } else {
                return Registry.CurrentUser.CreateSubKey(program.RegistryPath);
            }
        }

        public static RegistryKey GetWriteStartupRegistryKey(StartupProgram program) {
            if (!program.AllUsers) {
                return Registry.CurrentUser.CreateSubKey(StartupRegistryPath);
            } else {
                return Registry.LocalMachine.CreateSubKey(StartupRegistryPath);
            }
        }

        public static RegistryKey GetReadRegistryKey(string registryPath, bool currentUser) {
            if (currentUser) {
                return Registry.CurrentUser.OpenSubKey(registryPath);
            } else {
                return Registry.LocalMachine.OpenSubKey(registryPath);
            }
        }

        public static IEnumerable<RegistryKey> GetReadRegistryKeys(bool allUsers, params string[] registryKeys) {
            if (allUsers) {
                return registryKeys.Select(x => Registry.LocalMachine.OpenSubKey(x));
            } else {
                return registryKeys.Select(x => Registry.CurrentUser.OpenSubKey(x));
            }
        }

        public static void SetBytes(RegistryKey reg, string name, byte[] bytes) {
            reg.SetValue(name, bytes);
        }

        public static byte[] MakeDisabledBytes() {
            var startBytes = new byte[] { 0x03, 0x00, 0x00, 0x00 };
            var now = DateTime.Now.Ticks;
            var timeBytes = BitConverter.GetBytes(now);
            var bytes = startBytes.Concat(timeBytes).ToArray();
            return bytes;
        }

        public static byte[] MakeEnabledBytes() {
            var bytes = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            return bytes;
        }
    }
}