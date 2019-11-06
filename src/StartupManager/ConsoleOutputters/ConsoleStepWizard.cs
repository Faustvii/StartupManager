using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StartupManager.ConsoleOutputters {
    public static class ConsoleStepWizard {
        public static IEnumerable<ConsoleStep> UserWizard(string message, IEnumerable<ConsoleStep> steps) {
            Console.WriteLine(message);
            Console.WriteLine();
            foreach (var step in steps) {
                Console.Write(step.Message);
                step.UserValue = GetValueFromUser(step.UserValue, step.Message);
            }
            return steps;
        }

        private static object GetValueFromUser(object val, string message) {
            return val
            switch {
                FileInfo file => GetFileInfoFromUser(message),
                    string stringVal => Console.ReadLine(),
                    bool boolVal => PromptUserForBool("y", "n", message),
                    _ => val
            };
        }

        public static FileInfo GetFileInfoFromUser(string message) {
            var input = Console.ReadLine().Replace("\"", string.Empty);
            var file = new FileInfo(input);
            while (!file.Exists) {
                Console.WriteLine();
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "That file doesn't seem to exist, please try again");
                Console.Write(message);
                input = Console.ReadLine().Replace("\"", string.Empty);;
                file = new FileInfo(input);
            }
            return file;
        }

        public static bool PromptUserForBool(string trueVal, string falseVal, string message) {
            var validOptions = new [] { trueVal, falseVal };
            var key = Console.ReadKey().Key.ToString();
            while (!validOptions.Any(x => x.Equals(key, StringComparison.OrdinalIgnoreCase))) {
                Console.WriteLine();
                Console.WriteLine($"Try again '{key}' is not a valid input");
                Console.Write(message);
                key = Console.ReadKey().Key.ToString();
            }
            Console.WriteLine();
            return key.Equals(trueVal, StringComparison.OrdinalIgnoreCase);
        }
    }
}