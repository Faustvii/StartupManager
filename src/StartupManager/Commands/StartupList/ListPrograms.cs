namespace StartupManager.Commands.StartupList {
    public class ListPrograms {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool CurrentUser { get; set; }
        public bool Disabled { get; set; }
        public string RegistryPath { get; set; }
        public string RegistryName { get; set; }

        public ListPrograms(string name, string details, bool currentUser, bool disabled, string registryPath, string registryName)
        {
            Path = details;
            CurrentUser = currentUser;
            Disabled = disabled;
            RegistryPath = registryPath;
            RegistryName = registryName;
            Name = ParseName(name);
        }

        private string ParseName(string name)
        {
            var parsedName = name == string.Empty ? "(Default)" : name;
            if(string.IsNullOrWhiteSpace(parsedName)){
                parsedName = $"'{parsedName}'";
            }

            return parsedName;
        }
    }
}