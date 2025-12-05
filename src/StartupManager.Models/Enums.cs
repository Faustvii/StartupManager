namespace StartupManager.Models {
    public enum StateChange {
        Success,
        SameState,
        Unauthorized
    }

    public enum ProcessPriority {
        Idle = 0,
        BelowNormal = 1,
        Normal = 2,
        AboveNormal = 3,
        High = 4,
        Realtime = 5
    }
}