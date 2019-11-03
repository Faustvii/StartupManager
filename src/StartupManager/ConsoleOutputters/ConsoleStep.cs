namespace StartupManager.ConsoleOutputters {
    public class ConsoleStep {
        public string Id { get; set; }
        public string Message { get; set; }
        public object UserValue { get; set; }
        public ConsoleStep(string id, string message, object userValue) {
            this.Id = id;
            this.Message = message;
            this.UserValue = userValue;
        }
    }
}