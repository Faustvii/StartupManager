using System.IO;

namespace StartupManager.Models {
    public class StartupProgram {
        public string Name { get; set; }
        public FileInfo File { get; set; }
        public string Arguments { get; set; }
        public bool Administrator { get; set; }
        public bool AllUsers { get; set; }
        public ProcessPriority Priority { get; set; }
        public StartupProgram(string name, FileInfo file, string arguments, bool administrator, bool allUsers) {
            this.Name = name;
            this.File = file;
            this.Arguments = arguments;
            this.Administrator = administrator;
            this.AllUsers = allUsers;
            this.Priority = ProcessPriority.Normal;
        }

        public StartupProgram(string name, FileInfo file, string arguments, bool administrator, bool allUsers, ProcessPriority priority) {
            this.Name = name;
            this.File = file;
            this.Arguments = arguments;
            this.Administrator = administrator;
            this.AllUsers = allUsers;
            this.Priority = priority;
        }
    }
}