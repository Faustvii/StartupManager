using System;
using System.Linq;

namespace StartupManager.Helpers {
    public static class ColorCodesHelper {
        private const string Black = "\u001b[30m";
        private const string Red = "\u001b[31m";
        private const string Green = "\u001b[32m";
        private const string Yellow = "\u001b[33m";
        private const string Blue = "\u001b[34m";
        private const string Magenta = "\u001b[35m";
        private const string Cyan = "\u001b[36m";
        private const string White = "\u001b[37m";
        private const string Reset = "\u001b[0m";

        public static void ConsoleWriteLineColored(ConsoleColor color, params string[] text) {
            Console.ForegroundColor = color;
            foreach (var line in text) {
                System.Console.WriteLine(line);
            }
            Console.ResetColor();
        }

        public static void WriteToConsole(ConsoleColor color, params ProgramOutput[] programs) {
            foreach (var program in programs.Select(x => x.Format(programs.Max(n => n.Name.Length)))) {
                Console.ForegroundColor = color;
                Console.Write(program.Name);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {program.Details}");
            }
            Console.ResetColor();
        }

        public static void ConsoleWriteColored(ConsoleColor color, params string[] text) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var line in text) {
                System.Console.Write(line);
            }
            Console.ResetColor();
        }

        public static string Color(this string me, Colors color) {
            switch (color) {
                case Colors.Black:
                    return $"{Black}{me}{Reset}";
                case Colors.Blue:
                    return $"{Blue}{me}{Reset}";
                case Colors.Cyan:
                    return $"{Cyan}{me}{Reset}";
                case Colors.Green:
                    return $"{Green}{me}{Reset}";
                case Colors.Magenta:
                    return $"{Magenta}{me}{Reset}";
                case Colors.Red:
                    return $"{Red}{me}{Reset}";
                case Colors.White:
                    return $"{White}{me}{Reset}";
                case Colors.Yellow:
                    return $"{Yellow}{me}{Reset}";
                default:
                    return $"{me}";
            }
        }
    }

    public enum Colors {
        Black,
        Red,
        Green,
        Yellow,
        Blue,
        Magenta,
        Cyan,
        White
    }
}