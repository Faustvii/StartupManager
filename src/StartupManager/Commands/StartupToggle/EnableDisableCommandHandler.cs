using System;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters;
using StartupManager.Helpers;

namespace StartupManager.Commands.StartupToggle {
    public static class EnableDisableCommandHandler {
        public static void Run(string name, bool enable) {
            var toggleText = enable ? "enabled" : "disabled";
            var program = ListCommandHandler.Run(true).FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (program == null) {
                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Red, "Could not find a program with name ");
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Yellow, name);
                return;
            }

            switch (program.Type) {
                case ListProgram.StartupType.TaskScheduler:
                    ToggleThroughTaskScheduler(enable, toggleText, program);
                    break;
                case ListProgram.StartupType.Regedit:
                case ListProgram.StartupType.Shortcut:
                    ToggleThroughRegedit(enable, toggleText, program);
                    break;
            }
        }

        private static void ToggleThroughTaskScheduler(bool enable, string toggleText, ListProgram program) {
            try {
                using(var taskService = new TaskService()) {
                    var task = taskService.FindTask(program.Name);
                    var isAlreadyTheRequestState = task.Enabled == enable;
                    if (isAlreadyTheRequestState) {
                        WriteSameStateConsoleOutput(toggleText, program);
                    } else {
                        task.Enabled = enable;
                        WriteToggledConsoleOutput(toggleText, program);
                    }
                }
            } catch (UnauthorizedAccessException) {
                WriteRequireAdministratorConsoleOutput(program);
            }
        }

        private static void WriteSameStateConsoleOutput(string toggleText, ListProgram program) {
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
            Console.WriteLine($" is already {toggleText}");
        }

        private static void ToggleThroughRegedit(bool enable, string toggleText, ListProgram program) {
            try {
                using(var reg = RegistryHelper.GetWriteRegistryKey(program.DisabledRegistryPath, program.AllUsers)) {
                    byte[] bytes;
                    if (enable) {
                        bytes = RegistryHelper.MakeEnabledBytes();
                    } else {
                        bytes = RegistryHelper.MakeDisabledBytes();
                    }

                    var currentValue = (byte[])reg.GetValue(program.RegistryName);
                    var isAlreadyTheRequestState = new ReadOnlySpan<byte>(bytes.Take(4).ToArray()).SequenceEqual(currentValue.Take(4).ToArray());

                    if (isAlreadyTheRequestState) {
                        WriteSameStateConsoleOutput(toggleText, program);
                    } else {
                        RegistryHelper.SetBytes(reg, program.RegistryName, bytes);

                        WriteToggledConsoleOutput(toggleText, program);
                    }
                }
            } catch (UnauthorizedAccessException) {
                WriteRequireAdministratorConsoleOutput(program);
            }
        }

        private static void WriteRequireAdministratorConsoleOutput(ListProgram program) {
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Red, $"To modify settings for ");
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, " you need to run the command with administrator (sudo)");
        }

        private static void WriteToggledConsoleOutput(string toggleText, ListProgram program) {
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, program.Name);
            Console.WriteLine($" has been {toggleText}");
        }
    }
}