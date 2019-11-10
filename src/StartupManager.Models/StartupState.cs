namespace StartupManager.Models {
    public class StartupState {
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public StartupState(string name, bool disabled) {
            Name = name;
            Disabled = disabled;
        }
    }
}