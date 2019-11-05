using System;
using System.Collections.Generic;
using StartupManager.Commands.StartupList;
using StartupManager.Helpers.Tables;

namespace StartupManager.ConsoleOutputters.ListOutput {
    internal static class ListCommandOutputter {
        public static void WriteToConsole(bool detailed, IEnumerable<ListProgram> programs) {
            System.Console.WriteLine("Applications starting with windows:");
            Output(programs, detailed);
            Console.ResetColor();
        }

        private static void Output(IEnumerable<ListProgram> programs, bool detailed) {
            var tableHeaders = new List<Header<ListProgram>> {
                new Header<ListProgram>("Name", x => x.Name, x => ConsoleColor.Yellow),
                new Header<ListProgram>("Admin", x => x.RequireAdministrator ? "[\u221A]" : string.Empty, x => ConsoleColor.Cyan),
                new Header<ListProgram>("Enabled", x => x.Disabled ? "[x]" : "[\u221A]", x => x.Disabled ? ConsoleColor.Red : ConsoleColor.DarkCyan),
            };

            if (detailed) {
                tableHeaders.Add(new Header<ListProgram>("Path", x => x.Path, x => ConsoleColor.Green));
            }

            var tableModel = new Table<ListProgram>(programs, tableHeaders.ToArray());

            tableModel.OutputToConsoleColored();
        }
    }
}