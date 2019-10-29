using System;
using System.Collections.Generic;
using StartupManager.Commands.StartupList;
using StartupManager.Helpers.Tables;

namespace StartupManager.ConsoleOutputters.ListOutput {
    internal static class ListCommandOutputter {
        public static void WriteToConsole(bool detailed, IEnumerable<ListPrograms> programs) {
            System.Console.WriteLine("Applications starting with windows:");
            Output(programs, detailed);
            Console.ResetColor();
        }

        private static void Output(IEnumerable<ListPrograms> programs, bool detailed) {
            var tableHeaders = new List<Header<ListPrograms>> {
                new Header<ListPrograms>("Name", x => x.Name, x => ConsoleColor.Yellow),
                new Header<ListPrograms>("Admin", x => x.CurrentUser ? string.Empty : "[\u221A]", x => ConsoleColor.Cyan),
                new Header<ListPrograms>("Enabled", x => x.Disabled ? "[x]" : "[\u221A]", x => x.Disabled ? ConsoleColor.Red : ConsoleColor.DarkCyan),
            };

            if (detailed) {
                tableHeaders.Add(new Header<ListPrograms>("Path", x => x.Path, x => ConsoleColor.Green));
            }

            var tableModel = new Table<ListPrograms>(programs, tableHeaders.ToArray());

            tableModel.OutputToConsoleColored();
        }
    }
}