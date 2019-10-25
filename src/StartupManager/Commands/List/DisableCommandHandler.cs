using System;
using System.Linq;
using Microsoft.Win32;
namespace StartupManager.Commands.List {
    public static class DisableCommandHandler {
        private const string DisabledStartupRegistryItems = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private const string DisabledStartupFolderItems = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        public static void Run(string name, string type, bool allUsers) {
            using(var userFolder = Registry.CurrentUser.CreateSubKey(DisabledStartupFolderItems))
            using(var userReg = Registry.CurrentUser.CreateSubKey(DisabledStartupRegistryItems)) {
                byte[] bytes = MakeDisabledBytes();
                if (userReg.GetValueNames().Any(x => x == name)) {
                    Disable(userReg, name, bytes);
                } else if (userFolder.GetValueNames().Any(x => x == name)) {
                    Disable(userFolder, name, bytes);
                } else { }

            }
        }

        private static string DetermineRegistryKey(string type, bool allUsers) {
            switch (type) {
                case "test":
                default:
                    return DisabledStartupFolderItems;
            }
        }

        private static void Disable(RegistryKey reg, string name, byte[] bytes) {
            reg.SetValue(name, bytes);
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