using System.Security.Principal;

namespace StartupManager.Helpers {
    public static class IsElevatedHelper {
        public static bool IsElevated() {
            bool isElevated = false;
            using(WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return isElevated;
        }
    }
}