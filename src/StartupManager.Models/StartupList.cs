using System.Diagnostics.CodeAnalysis;

namespace StartupManager.Models {
    public class StartupList {
        private string _name = string.Empty;
        public string Name { get => _name; set => _name = ParseName(value); }
        public string Path { get; set; }
        public bool RequireAdministrator { get; set; }
        public bool Disabled { get; set; }
        public bool AllUsers { get; set; }
        public string RegistryPath { get; set; }
        public string DisabledRegistryPath { get; set; }
        public string RegistryName { get; set; }
        public StartupType Type { get; set; }
        public int Index { get; set; }
        public ProcessPriority? Priority { get; set; }

        public StartupList(string name, [AllowNull] string path, bool requireAdministrator, bool disabled, StartupType type, bool allUsers, string registryPath, string disabledRegistryPath, string registryName) {
            Path = path ?? string.Empty;
            RequireAdministrator = requireAdministrator;
            Disabled = disabled;
            Type = type;
            AllUsers = allUsers;
            Name = name;

            RegistryPath = registryPath;
            DisabledRegistryPath = disabledRegistryPath;
            RegistryName = registryName;
            Priority = null;
        }

        public StartupList(string name, [AllowNull] string path, bool requireAdministrator, bool disabled, StartupType type, bool allUsers) {
            Path = path ?? string.Empty;
            RequireAdministrator = requireAdministrator;
            Disabled = disabled;
            Type = type;
            AllUsers = allUsers;
            Name = name;

            RegistryPath = string.Empty;
            DisabledRegistryPath = string.Empty;
            RegistryName = string.Empty;
            Priority = null;
        }

        public StartupList(string name, [AllowNull] string path, bool requireAdministrator, bool disabled, StartupType type, bool allUsers, ProcessPriority? priority) {
            Path = path ?? string.Empty;
            RequireAdministrator = requireAdministrator;
            Disabled = disabled;
            Type = type;
            AllUsers = allUsers;
            Name = name;

            RegistryPath = string.Empty;
            DisabledRegistryPath = string.Empty;
            RegistryName = string.Empty;
            Priority = priority;
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
} //00:00:00.0074059