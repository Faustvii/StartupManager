using System.IO;
namespace StartupManager.Commands.Add {
    public class StartupProgram {
        public string Name { get; set; }
        public FileInfo File { get; set; }
        public string Arguments { get; set; }
        public bool Administrator { get; set; }
        public bool AllUsers { get; set; }
        public StartupProgram(string name, FileInfo file, string arguments, bool administrator, bool allUsers) {
            this.Name = name;
            this.File = file;
            this.Arguments = arguments;
            this.Administrator = administrator;
            this.AllUsers = allUsers;
        }
    }
}