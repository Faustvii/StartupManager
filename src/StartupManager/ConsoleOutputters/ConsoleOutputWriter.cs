using System;
using System.Collections.Generic;
using StartupManager.ConsoleOutputters.ListOutput;
using StartupManager.Models;

namespace StartupManager.ConsoleOutputters {
    public static class ConsoleOutputWriter {
        public static void WriteToConsole(bool detailed, IEnumerable<StartupList> programs) {
            ListCommandOutputter.WriteToConsole(detailed, programs);
        }

        public static void WriteToConsole(IEnumerable<ConsoleColorOutput> messages) {
            foreach (var message in messages) {
                WriteToConsole(message);
            }
        }

        private static void WriteToConsole(ConsoleColorOutput message) {
            switch (message.OutputMode) {
                case WriteMode.Write:
                    if (message.Color.HasValue) {
                        ConsoleColorHelper.ConsoleWriteColored(message.Color.Value, message.Message);
                    } else {
                        Console.Write(message.Message);
                    }
                    break;
                case WriteMode.Writeline:
                    if (message.Color.HasValue) {
                        ConsoleColorHelper.ConsoleWriteLineColored(message.Color.Value, message.Message);
                    } else {
                        Console.WriteLine(message.Message);
                    }
                    break;
            }
        }
    }
}