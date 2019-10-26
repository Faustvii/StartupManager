namespace StartupManager.Commands.StartupList {
    public class ListPrograms {
        public string Name { get; set; }
        public string Details { get; set; }
        public bool CurrentUser { get; set; }
        public bool Disabled { get; set; }
        public string RegistryPath { get; set; }
        public string RegistryName { get; set; }

        public ListPrograms(string name, string details, bool currentUser, bool disabled, string registryPath, string registryName) {
            Details = details;
            CurrentUser = currentUser;
            Disabled = disabled;
            RegistryPath = registryPath;
            RegistryName = registryName;
            Name = name;
        }
    }
}