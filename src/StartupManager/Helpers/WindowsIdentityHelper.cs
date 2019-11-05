using System.Security.Principal;

namespace StartupManager.Helpers {
    public static class WindowsIdentityHelper {
        public static bool IsElevated() {
            using(WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                var isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                return isElevated;
            }
        }

        public static string CurrentUser() {
            using(WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return identity.Name;
            }
        }
    }
}