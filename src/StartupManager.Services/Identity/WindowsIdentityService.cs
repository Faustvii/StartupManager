using System.Security.Principal;

namespace StartupManager.Services.Identity {
    public class WindowsIdentityService : IWindowsIdentityService {
        public bool IsElevated() {
            using(WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                var isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                return isElevated;
            }
        }

        public string CurrentUser() {
            using(WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return identity.Name;
            }
        }
    }
}