namespace StartupManager.Helpers {
    public class ProgramOutput {
        public string Name { get; set; }
        public string Details { get; set; }
        public bool AllUsers { get; set; }

        public bool Disabled { get; set; }

        public ProgramOutput(string name, string details, bool allUsers, bool disabled) {
            Name = name;
            Details = details;
            AllUsers = allUsers;
            Disabled = disabled;
        }

        public ProgramOutput Format(int spaceDividers) {
            return new ProgramOutput(Name.PadRight(spaceDividers), Details, AllUsers, Disabled);
        }
    }
}