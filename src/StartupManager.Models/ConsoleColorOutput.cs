using System;
namespace StartupManager.Models {
    public class ConsoleColorOutput {
        public readonly ConsoleColor? Color;
        public readonly WriteMode OutputMode;
        public readonly string Message;

        public ConsoleColorOutput(WriteMode outputMode, string message) {
            OutputMode = outputMode;
            Message = message;
        }

        public ConsoleColorOutput(WriteMode outputMode, string message, ConsoleColor color) {
            OutputMode = outputMode;
            Message = message;
            Color = color;
        }
    }

    public enum WriteMode {
        Write,
        Writeline
    }
}