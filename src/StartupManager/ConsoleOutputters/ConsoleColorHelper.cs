using System;
namespace StartupManager.ConsoleOutputters {
    internal static class ConsoleColorHelper {
        public static void ConsoleWriteLineColored(ConsoleColor color, params string[] data) {
            Console.ForegroundColor = color;
            foreach (var line in data) {
                Console.WriteLine(line);
            }
            Console.ResetColor();
        }

        public static void ConsoleWriteColored(ConsoleColor color, params string[] data) {
            Console.ForegroundColor = color;
            foreach (var line in data) {
                Console.Write(line);
            }
            Console.ResetColor();
        }
    }
}