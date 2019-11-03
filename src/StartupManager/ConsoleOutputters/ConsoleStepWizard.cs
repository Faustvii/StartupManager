using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StartupManager.ConsoleOutputters {
    public static class ConsoleStepWizard {
        public static IEnumerable<ConsoleStep> UserWizard(string message, IEnumerable<ConsoleStep> steps) {
            System.Console.WriteLine(message);
            foreach (var step in steps) {
                Console.Write(step.Message);
                step.UserValue = GetValueFromUser(step.UserValue);
            }
            return steps;
        }

        private static object GetValueFromUser(object val) {
            return val
            switch {
                FileInfo file => GetFileInfoFromUser(),
                    string stringVal => Console.ReadLine(),
                    bool boolVal => PromptUserForBool("y", "n"),
                    _ => val
            };
        }

        public static FileInfo GetFileInfoFromUser() {
            var input = Console.ReadLine().Replace("\"", string.Empty);
            var file = new FileInfo(input);
            while (!file.Exists) {
                System.Console.WriteLine();
                System.Console.WriteLine("That file doesn't seem to exist, please try again");
                input = Console.ReadLine().Replace("\"", string.Empty);;
                file = new FileInfo(input);
            }
            return file;
        }

        public static bool PromptUserForBool(string trueVal, string falseVal) {
            var validOptions = new [] { trueVal, falseVal };
            var key = Console.ReadKey().Key.ToString();
            while (!validOptions.Any(x => x.Equals(key, StringComparison.OrdinalIgnoreCase))) {
                System.Console.WriteLine();
                System.Console.WriteLine($"Try again '{key}' is not a valid input");
                key = Console.ReadKey().Key.ToString();
            }
            System.Console.WriteLine();
            return key.Equals(trueVal, StringComparison.OrdinalIgnoreCase);
        }
    }
}