namespace StartupManager.Services.Identity {
    public interface IWindowsIdentityService {
        string CurrentUser();
        bool IsElevated();
    }
}