using System.Diagnostics.CodeAnalysis;

namespace StartupManager.Commands.StartupList {
    public class ListProgram {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool RequireAdministrator { get; set; }
        public bool Disabled { get; set; }
        public bool AllUsers { get; set; }
        public string RegistryPath { get; set; }
        public string DisabledRegistryPath { get; set; }
        public string RegistryName { get; set; }
        public StartupType Type { get; set; }

        public ListProgram(string name, [AllowNull] string path, bool requireAdministrator, bool disabled, StartupType type, bool allUsers, string registryPath = "", string disabledRegistryPath = "", string registryName = "") {
            Path = path ?? string.Empty;
            RequireAdministrator = requireAdministrator;
            Disabled = disabled;
            Type = type;
            AllUsers = allUsers;
            RegistryPath = registryPath;
            DisabledRegistryPath = disabledRegistryPath;
            RegistryName = registryName;
            Name = ParseName(name);
        }

        private string ParseName(string name) {
            var parsedName = name == string.Empty ? "(Default)" : name;
            if (string.IsNullOrWhiteSpace(parsedName)) {
                parsedName = $"'{parsedName}'";
            }

            return parsedName;
        }

        public enum StartupType {
            Shortcut,
            Regedit,
            TaskScheduler
        }
    }
}