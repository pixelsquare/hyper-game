using System.Collections;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitSignOut : WaitUnit
    {
        protected override IEnumerator Await(Flow flow)
        {    
            Services.AuthService.LogOutUserAsync(new LogoutUserRequest());
            ConnectionManager.Client.Disconnect();
            SceneManager.LoadScene(SceneNames.SIGNUP_SCENE);
            BackendUtil.ClearAllTokens();
            yield return exit;
        }
    }
}
