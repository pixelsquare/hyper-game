using System.Collections;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitAuthLogout : WaitUnit
    {
        protected override IEnumerator Await(Flow flow)
        {
            SignOutManager.Instance.ResetAppSettings();
            Services.AuthService.LogOutUserAsync(new LogoutUserRequest());
            yield return exit;
        }
    }
}
