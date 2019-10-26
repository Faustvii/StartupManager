using System;
using System.Linq;
using Microsoft.Win32;
using StartupManager.Commands.StartupList;

namespace StartupManager.Helpers {
    public static class RegistryHelper {
        public static RegistryKey GetRegistryKey(ListPrograms program) {
            if (program.AllUsers) {
                return Registry.LocalMachine.CreateSubKey(program.RegistryPath);
            } else {
                return Registry.CurrentUser.CreateSubKey(program.RegistryPath);
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