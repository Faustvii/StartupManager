using System;
using System.Collections.Generic;
using System.Linq;
using StartupManager.Helpers;

namespace StartupManager.ConsoleOutputters {
    internal static class ListCommandOutputter {
        public static void WriteToConsole(bool detailed, IEnumerable<ProgramOutput> programs) {
            System.Console.WriteLine("Applications starting with windows:");
            if (detailed) {
                DetailedOutput(programs);
            } else {
                Console.WriteLine();
                ColorCodesHelper.ConsoleWriteLineColored(ConsoleColor.Yellow, programs.Select(x => x.Name).ToArray());
            }
            Console.ResetColor();
        }

        private static void DetailedOutput(IEnumerable<ProgramOutput> programs) {
            var maxLength = programs.Max(x => x.Name.Length);
            var seperator = new string(' ', 2);
            var tableHeader = "Name".PadRight(maxLength);
            tableHeader = $"{tableHeader}Enabled{seperator}Location{Environment.NewLine}";
            ColorCodesHelper.ConsoleWriteLineColored(ConsoleColor.DarkGreen, tableHeader);
            foreach (var program in programs) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(program.Name.PadRight(maxLength));
                Console.ForegroundColor = program.Disabled ? ConsoleColor.Red : ConsoleColor.DarkCyan;
                Console.Write($"  [{(program.Disabled ? "x" : "\u221A")}]".PadRight(7));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{seperator}{program.Details}");
            }
        }
    }
}