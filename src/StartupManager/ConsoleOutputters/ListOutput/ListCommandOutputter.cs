using System;
using System.Collections.Generic;
using System.Linq;
using StartupManager.Commands.StartupList;
using StartupManager.Helpers.Tables;

namespace StartupManager.ConsoleOutputters.ListOutput {
    internal static class ListCommandOutputter {
        public static void WriteToConsole(bool detailed, IEnumerable<ListPrograms> programs) {
            System.Console.WriteLine("Applications starting with windows:");
            if (detailed) {
                DetailedOutput(programs);
            } else {
                Console.WriteLine();
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Yellow, programs.Select(x => x.Name).ToArray());
            }
            Console.ResetColor();
        }

        private static void DetailedOutput(IEnumerable<ListPrograms> programs) {
            var tableHeaders = new [] {
                new Header<ListPrograms>("Name", x => x.Name, x => ConsoleColor.Yellow),
                new Header<ListPrograms>("Admin", x => x.AllUsers ? "[\u221A]" : string.Empty, x => ConsoleColor.Cyan),
                new Header<ListPrograms>("Enabled", x => x.Disabled ? "[x]" : "[\u221A]", x => x.Disabled ? ConsoleColor.Red : ConsoleColor.DarkCyan),
                new Header<ListPrograms>("Location", x => x.Details, x => ConsoleColor.Green),
            };

            var tableModel = new Table<ListPrograms>(programs, tableHeaders);

            tableModel.OutputToConsoleColored();
        }
    }
}