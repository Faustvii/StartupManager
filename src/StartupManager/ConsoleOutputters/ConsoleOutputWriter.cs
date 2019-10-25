using System.Collections.Generic;
using StartupManager.Helpers;

namespace StartupManager.ConsoleOutputters {
    public static class ConsoleOutputWriter {
        public static void WriteToConsole(bool detailed, IEnumerable<ProgramOutput> programs) {
            ListCommandOutputter.WriteToConsole(detailed, programs);
        }
    }
}