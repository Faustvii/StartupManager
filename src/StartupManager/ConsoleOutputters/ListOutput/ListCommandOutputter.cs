using System;
using System.Collections.Generic;
using StartupManager.ConsoleOutputters.Tables;
using StartupManager.Models;

namespace StartupManager.ConsoleOutputters.ListOutput {
    internal static class ListCommandOutputter {
        public static void WriteToConsole(bool detailed, IEnumerable<StartupList> programs) {
            System.Console.WriteLine("Applications starting with windows:");
            Output(programs, detailed);
            Console.ResetColor();
        }

        private static void Output(IEnumerable<StartupList> programs, bool detailed) {
            var tableHeaders = new List<Header<StartupList>> {
                new Header<StartupList>("#", x => x.Index, x => ConsoleColor.Blue),
                new Header<StartupList>("Name", x => x.Name, x => ConsoleColor.Yellow),
                new Header<StartupList>("Admin", x => x.RequireAdministrator ? "[\u221A]" : string.Empty, x => ConsoleColor.Cyan),
                new Header<StartupList>("Enabled", x => x.Disabled ? "[x]" : "[\u221A]", x => x.Disabled ? ConsoleColor.Red : ConsoleColor.DarkCyan),
                new Header<StartupList>("Priority", x => x.Priority?.ToString() ?? "N/A", x => x.Priority.HasValue ? ConsoleColor.Magenta : ConsoleColor.DarkGray),
            };

            if (detailed) {
                tableHeaders.Add(new Header<StartupList>("Path", x => x.Path, x => ConsoleColor.Green));
            }

            var tableModel = new Table<StartupList>(programs, tableHeaders.ToArray());

            tableModel.OutputToConsoleColored();
        }
    }
}