using System.Collections.Generic;
using StartupManager.Commands.StartupList;
using StartupManager.ConsoleOutputters.ListOutput;

namespace StartupManager.ConsoleOutputters {
    public static class ConsoleOutputWriter {
        public static void WriteToConsole(bool detailed, IEnumerable<ListProgram> programs) {
            ListCommandOutputter.WriteToConsole(detailed, programs);
        }
    }
}